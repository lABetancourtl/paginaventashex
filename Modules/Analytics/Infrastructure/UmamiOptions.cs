namespace PaginaVentasNet.Api.Modules.Analytics.Infrastructure;

/// <summary>Configuración de Umami, leída de las variables de entorno.</summary>
public class UmamiOptions
{
    public string BaseUrl { get; init; } = string.Empty;
    public string WebsiteId { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Nombre del tipo de métrica para las rutas: "path" en Umami v3, "url" en v2.
    /// </summary>
    public string PathMetricType { get; init; } = "path";

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(BaseUrl)
        && !string.IsNullOrWhiteSpace(WebsiteId)
        && !string.IsNullOrWhiteSpace(Username)
        && !string.IsNullOrWhiteSpace(Password);

    public static UmamiOptions FromEnvironment() => new()
    {
        BaseUrl = (Environment.GetEnvironmentVariable("UMAMI_URL") ?? string.Empty).TrimEnd('/'),
        WebsiteId = Environment.GetEnvironmentVariable("UMAMI_WEBSITE_ID") ?? string.Empty,
        Username = Environment.GetEnvironmentVariable("UMAMI_USERNAME") ?? string.Empty,
        Password = Environment.GetEnvironmentVariable("UMAMI_PASSWORD") ?? string.Empty,
        PathMetricType = Environment.GetEnvironmentVariable("UMAMI_PATH_METRIC_TYPE") is { Length: > 0 } t
            ? t : "path"
    };
}
