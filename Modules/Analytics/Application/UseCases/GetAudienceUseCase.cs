using PaginaVentasNet.Api.Modules.Analytics.Application.Dtos;
using PaginaVentasNet.Api.Modules.Analytics.Application.Ports;
using PaginaVentasNet.Api.Modules.Analytics.Domain;

namespace PaginaVentasNet.Api.Modules.Analytics.Application.UseCases;

/// <summary>
/// Quiénes visitan la tienda: dispositivo (celular, computador, tablet), países y ciudades.
/// La ubicación sale de la IP: las visitas desde la red local quedan como desconocidas.
/// </summary>
public class GetAudienceUseCase
{
    private const int TopLocations = 10;

    private readonly IAnalyticsProvider _provider;

    public GetAudienceUseCase(IAnalyticsProvider provider)
    {
        _provider = provider;
    }

    public async Task<AudienceDto> ExecuteAsync(DateOnly? from, DateOnly? to)
    {
        var range = AnalyticsRange.Create(from, to);

        var devices = await _provider.GetMetricAsync(range.StartUtc, range.EndUtc, AnalyticsMetric.Device, 20);
        var countries = await _provider.GetMetricAsync(range.StartUtc, range.EndUtc, AnalyticsMetric.Country, TopLocations);
        var cities = await _provider.GetMetricAsync(range.StartUtc, range.EndUtc, AnalyticsMetric.City, TopLocations);

        // Toda visita tiene dispositivo: su suma es el total de visitantes del periodo
        var total = devices.Sum(d => d.Visitors);

        var groupedDevices = devices
            .GroupBy(d => DeviceGroup(d.Value))
            .Select(g => new MetricCount(g.Key, g.Sum(d => d.Visitors)));

        return new AudienceDto
        {
            TotalVisitors = total,
            Devices = ToItems(groupedDevices, total),
            Countries = ToItems(countries, total),
            Cities = ToItems(cities, total)
        };
    }

    /// <summary>Umami distingue laptop y desktop; para la tienda los dos son "computador".</summary>
    private static string DeviceGroup(string device) => device.ToLowerInvariant() switch
    {
        "mobile" => "mobile",
        "tablet" => "tablet",
        "laptop" or "desktop" => "computer",
        _ => string.Empty
    };

    private static List<AudienceItemDto> ToItems(IEnumerable<MetricCount> metrics, long total) => metrics
        .Where(m => m.Visitors > 0)
        .OrderByDescending(m => m.Visitors)
        .Select(m => new AudienceItemDto
        {
            Value = m.Value,
            Visitors = m.Visitors,
            Percent = total == 0 ? 0 : Math.Round(100.0 * m.Visitors / total, 1)
        })
        .ToList();
}
