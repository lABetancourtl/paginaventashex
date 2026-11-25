using PaginaVentasNet.Api.Modules.Analytics.Application.Dtos;
using PaginaVentasNet.Api.Modules.Analytics.Application.Ports;
using PaginaVentasNet.Api.Modules.Analytics.Domain;

namespace PaginaVentasNet.Api.Modules.Analytics.Application.UseCases;

/// <summary>
/// Visitantes, visitas, páginas vistas, duración promedio y rebote del periodo,
/// junto con el periodo anterior de la misma longitud.
/// </summary>
public class GetTrafficSummaryUseCase
{
    private readonly IAnalyticsProvider _provider;

    public GetTrafficSummaryUseCase(IAnalyticsProvider provider)
    {
        _provider = provider;
    }

    public async Task<TrafficSummaryDto> ExecuteAsync(DateOnly? from, DateOnly? to)
    {
        var range = AnalyticsRange.Create(from, to);
        var previous = range.Previous();

        var current = await _provider.GetStatsAsync(range.StartUtc, range.EndUtc);
        var before = await _provider.GetStatsAsync(previous.StartUtc, previous.EndUtc);

        return new TrafficSummaryDto
        {
            From = range.From,
            To = range.To,
            Current = ToTotals(current),
            Previous = ToTotals(before)
        };
    }

    private static TrafficTotalsDto ToTotals(TrafficStats s) => new()
    {
        Visitors = s.Visitors,
        Visits = s.Visits,
        Pageviews = s.Pageviews,
        AvgVisitDurationSeconds = s.Visits == 0 ? 0 : Math.Round((double)s.TotalTimeSeconds / s.Visits, 1),
        BounceRate = s.Visits == 0 ? 0 : Math.Round(100.0 * Math.Min(s.Bounces, s.Visits) / s.Visits, 1),
        PagesPerVisit = s.Visits == 0 ? 0 : Math.Round((double)s.Pageviews / s.Visits, 2)
    };
}
