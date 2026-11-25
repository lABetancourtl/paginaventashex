using PaginaVentasNet.Api.Modules.Analytics.Application.Dtos;
using PaginaVentasNet.Api.Modules.Analytics.Application.Ports;
using PaginaVentasNet.Api.Modules.Analytics.Domain;

namespace PaginaVentasNet.Api.Modules.Analytics.Application.UseCases;

/// <summary>
/// A qué hora entra la gente: suma el tráfico del rango en 24 franjas (hora de Colombia).
/// </summary>
public class GetTrafficByHourUseCase
{
    private readonly IAnalyticsProvider _provider;

    public GetTrafficByHourUseCase(IAnalyticsProvider provider)
    {
        _provider = provider;
    }

    public async Task<List<HourBucketDto>> ExecuteAsync(DateOnly? from, DateOnly? to)
    {
        var range = AnalyticsRange.Create(from, to);
        var points = await _provider.GetSeriesAsync(range.StartUtc, range.EndUtc, TimeUnit.Hour);

        var buckets = Enumerable.Range(0, 24)
            .Select(h => new HourBucketDto { Hour = h })
            .ToArray();

        foreach (var point in points)
        {
            buckets[point.Date.Hour].Visitors += point.Visitors;
            buckets[point.Date.Hour].Pageviews += point.Pageviews;
        }

        return buckets.ToList();
    }
}
