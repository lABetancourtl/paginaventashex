using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Analytics.Application.Ports;
using PaginaVentasNet.Api.Modules.Analytics.Domain;

namespace PaginaVentasNet.Api.Modules.Analytics.Infrastructure;

/// <summary>
/// Lee el tráfico desde la API de Umami. Inicia sesión con un usuario de solo lectura
/// y guarda el token en memoria; si Umami lo rechaza (401), vuelve a iniciar sesión una vez.
/// Todo lo que depende de la versión de Umami vive en esta clase.
/// </summary>
public class UmamiAnalyticsProvider : IAnalyticsProvider
{
    private const string TokenCacheKey = "umami:token";
    private const int PathMetricLimit = 1000;

    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private readonly UmamiOptions _options;
    private readonly ILogger<UmamiAnalyticsProvider> _logger;

    public UmamiAnalyticsProvider(
        HttpClient http,
        IMemoryCache cache,
        UmamiOptions options,
        ILogger<UmamiAnalyticsProvider> logger)
    {
        _http = http;
        _cache = cache;
        _options = options;
        _logger = logger;
    }

    public async Task<TrafficStats> GetStatsAsync(DateTime startUtc, DateTime endUtc)
    {
        using var doc = (await GetAsync($"stats?{RangeQuery(startUtc, endUtc)}"))!;
        var root = doc.RootElement;

        return new TrafficStats(
            Pageviews: ReadStat(root, "pageviews"),
            Visitors: ReadStat(root, "visitors"),
            Visits: ReadStat(root, "visits"),
            Bounces: ReadStat(root, "bounces"),
            TotalTimeSeconds: ReadStat(root, "totaltime"));
    }

    public async Task<List<TrafficPoint>> GetSeriesAsync(DateTime startUtc, DateTime endUtc, TimeUnit unit)
    {
        var unitName = unit.ToString().ToLowerInvariant();
        var query = $"{RangeQuery(startUtc, endUtc)}&unit={unitName}" +
                    $"&timezone={Uri.EscapeDataString(AnalyticsRange.TimeZoneId)}";

        using var doc = (await GetAsync($"pageviews?{query}"))!;
        var root = doc.RootElement;

        var pageviews = ReadSeries(root, "pageviews");
        var sessions = ReadSeries(root, "sessions");

        return pageviews.Keys.Union(sessions.Keys)
            .OrderBy(d => d)
            .Select(d => new TrafficPoint(
                d,
                pageviews.GetValueOrDefault(d),
                sessions.GetValueOrDefault(d)))
            .ToList();
    }

    public async Task<List<PathViews>> GetPathViewsAsync(DateTime startUtc, DateTime endUtc)
    {
        var query = $"{RangeQuery(startUtc, endUtc)}&type={_options.PathMetricType}&limit={PathMetricLimit}";

        // Umami v3: /metrics devuelve visitantes únicos por ruta; las páginas vistas están en /metrics/expanded
        using (var expanded = await GetAsync($"metrics/expanded?{query}", allowNotFound: true))
        {
            if (expanded is not null && expanded.RootElement.ValueKind == JsonValueKind.Array)
            {
                return expanded.RootElement.EnumerateArray()
                    .Select(e => new PathViews(
                        e.TryGetProperty("name", out var n) ? n.GetString() ?? string.Empty : string.Empty,
                        e.TryGetProperty("pageviews", out var pv) ? ReadNumber(pv) : 0))
                    .ToList();
            }
        }

        // Umami v2: /metrics ya devuelve páginas vistas
        using var doc = (await GetAsync($"metrics?{query}"))!;

        if (doc.RootElement.ValueKind != JsonValueKind.Array)
            return new List<PathViews>();

        return doc.RootElement.EnumerateArray()
            .Select(e => new PathViews(
                e.TryGetProperty("x", out var x) ? x.GetString() ?? string.Empty : string.Empty,
                e.TryGetProperty("y", out var y) ? ReadNumber(y) : 0))
            .ToList();
    }

    public async Task<List<MetricCount>> GetMetricAsync(
        DateTime startUtc, DateTime endUtc, AnalyticsMetric metric, int limit)
    {
        var type = metric.ToString().ToLowerInvariant();
        using var doc = (await GetAsync($"metrics?{RangeQuery(startUtc, endUtc)}&type={type}&limit={limit}"))!;

        if (doc.RootElement.ValueKind != JsonValueKind.Array)
            return new List<MetricCount>();

        return doc.RootElement.EnumerateArray()
            .Select(e => new MetricCount(
                e.TryGetProperty("x", out var x) && x.ValueKind == JsonValueKind.String ? x.GetString()! : string.Empty,
                e.TryGetProperty("y", out var y) ? ReadNumber(y) : 0))
            .ToList();
    }

    public async Task<long> GetActiveVisitorsAsync()
    {
        using var doc = (await GetAsync("active"))!;
        var root = doc.RootElement;

        // v3: { "visitors": 3 }; v2: { "x": 3 }
        if (root.ValueKind == JsonValueKind.Object)
        {
            if (root.TryGetProperty("visitors", out var visitors)) return ReadNumber(visitors);
            if (root.TryGetProperty("x", out var x)) return ReadNumber(x);
        }

        return 0;
    }

    // ---------- HTTP ----------

    /// <summary>Con allowNotFound, un 404 devuelve null (endpoint que no existe en esa versión de Umami).</summary>
    private async Task<JsonDocument?> GetAsync(string relativePath, bool allowNotFound = false)
    {
        if (!_options.IsConfigured)
            throw Unavailable("Las estadísticas no están configuradas en el servidor.");

        var url = $"{_options.BaseUrl}/api/websites/{_options.WebsiteId}/{relativePath}";

        try
        {
            var response = await SendAsync(url, await GetTokenAsync());

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response.Dispose();
                _cache.Remove(TokenCacheKey);
                response = await SendAsync(url, await GetTokenAsync());
            }

            using (response)
            {
                if (allowNotFound && response.StatusCode == HttpStatusCode.NotFound)
                    return null;

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Umami respondió {Status} para {Path}", (int)response.StatusCode, relativePath);
                    throw Unavailable("No se pudieron obtener las estadísticas.");
                }

                await using var stream = await response.Content.ReadAsStreamAsync();
                return await JsonDocument.ParseAsync(stream);
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning(ex, "No se pudo consultar Umami ({Path})", relativePath);
            throw Unavailable("El servicio de estadísticas no está disponible.");
        }
    }

    private async Task<HttpResponseMessage> SendAsync(string url, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _http.SendAsync(request);
    }

    private async Task<string> GetTokenAsync()
    {
        if (_cache.TryGetValue(TokenCacheKey, out string? cached) && cached is not null)
            return cached;

        // Cuerpo con Content-Length (no chunked): algunos proxies no aceptan cuerpos por partes
        var body = JsonSerializer.Serialize(new { username = _options.Username, password = _options.Password });
        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        using var response = await _http.PostAsync($"{_options.BaseUrl}/api/auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Umami rechazó el inicio de sesión: {Status}", (int)response.StatusCode);
            throw Unavailable("No se pudo iniciar sesión en el servicio de estadísticas.");
        }

        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var token = doc.RootElement.GetProperty("token").GetString()
                    ?? throw Unavailable("El servicio de estadísticas no devolvió un token.");

        _cache.Set(TokenCacheKey, token, TimeSpan.FromHours(12));
        return token;
    }

    // ---------- Lectura de la respuesta ----------

    private static string RangeQuery(DateTime startUtc, DateTime endUtc) =>
        $"startAt={ToUnixMs(startUtc)}&endAt={ToUnixMs(endUtc)}";

    private static long ToUnixMs(DateTime utc) =>
        new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc)).ToUnixTimeMilliseconds();

    /// <summary>Umami v2 devuelve { "pageviews": { "value": 10, "prev": 8 } }; v3 devuelve { "pageviews": 10 }.</summary>
    private static long ReadStat(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var value))
            return 0;

        if (value.ValueKind == JsonValueKind.Object && value.TryGetProperty("value", out var inner))
            return ReadNumber(inner);

        return ReadNumber(value);
    }

    private static long ReadNumber(JsonElement e) => e.ValueKind switch
    {
        JsonValueKind.Number => e.TryGetInt64(out var l) ? l : (long)e.GetDouble(),
        JsonValueKind.String when long.TryParse(e.GetString(), out var s) => s,
        _ => 0
    };

    /// <summary>Lee [{ "x": "2026-11-20 14:00:00", "y": 5 }].</summary>
    private static Dictionary<DateTime, long> ReadSeries(JsonElement root, string name)
    {
        var result = new Dictionary<DateTime, long>();

        if (!root.TryGetProperty(name, out var array) || array.ValueKind != JsonValueKind.Array)
            return result;

        foreach (var point in array.EnumerateArray())
        {
            if (!point.TryGetProperty("x", out var x) || x.GetString() is not { } text)
                continue;

            if (!TryParseLocalDate(text, out var date))
                continue;

            var y = point.TryGetProperty("y", out var yValue) ? ReadNumber(yValue) : 0;
            result[date] = result.GetValueOrDefault(date) + y;
        }

        return result;
    }

    /// <summary>
    /// Umami ya devuelve la hora "de pared" de la zona pedida (America/Bogota): v2 sin zona
    /// ("2026-11-23 22:00:00") y v3 con una "Z" que NO significa UTC ("2026-11-23T22:00:00Z").
    /// Por eso se toman fecha y hora tal cual y se ignora cualquier indicador de zona.
    /// </summary>
    private static bool TryParseLocalDate(string text, out DateTime date)
    {
        var wallClock = text.Length >= 19 ? text[..19].Replace('T', ' ') : text;

        if (DateTime.TryParseExact(wallClock, new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd" },
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var local))
        {
            date = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
            return true;
        }

        date = default;
        return false;
    }

    private static AppException Unavailable(string message) =>
        new("ANALYTICS_UNAVAILABLE", message, 503);
}
