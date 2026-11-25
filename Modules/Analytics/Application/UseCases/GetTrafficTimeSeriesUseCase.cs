using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Analytics.Application.Dtos;
using PaginaVentasNet.Api.Modules.Analytics.Application.Ports;
using PaginaVentasNet.Api.Modules.Analytics.Domain;

namespace PaginaVentasNet.Api.Modules.Analytics.Application.UseCases;

/// <summary>Visitantes y páginas vistas por hora, día o mes, para la gráfica de línea.</summary>
public class GetTrafficTimeSeriesUseCase
{
    private const int MaxDaysByHour = 31;

    private readonly IAnalyticsProvider _provider;

    public GetTrafficTimeSeriesUseCase(IAnalyticsProvider provider)
    {
        _provider = provider;
    }

    public async Task<List<TimeSeriesPointDto>> ExecuteAsync(DateOnly? from, DateOnly? to, TimeUnit unit)
    {
        var range = AnalyticsRange.Create(from, to);

        if (unit == TimeUnit.Hour && range.To.DayNumber - range.From.DayNumber + 1 > MaxDaysByHour)
            throw new AppException("ANALYTICS_INVALID_RANGE",
                $"La vista por hora admite hasta {MaxDaysByHour} días.", 400);

        var points = await _provider.GetSeriesAsync(range.StartUtc, range.EndUtc, unit);
        var byDate = points.ToDictionary(p => p.Date);

        // Se rellenan los intervalos sin visitas con cero para que la gráfica no salte
        return EnumerateIntervals(range, unit)
            .Select(date =>
            {
                byDate.TryGetValue(date, out var point);
                return new TimeSeriesPointDto
                {
                    Date = date,
                    Visitors = point?.Visitors ?? 0,
                    Pageviews = point?.Pageviews ?? 0
                };
            })
            .ToList();
    }

    private static IEnumerable<DateTime> EnumerateIntervals(AnalyticsRange range, TimeUnit unit)
    {
        var start = range.From.ToDateTime(TimeOnly.MinValue);
        var end = range.To.ToDateTime(TimeOnly.MaxValue);

        if (unit == TimeUnit.Month)
            start = new DateTime(start.Year, start.Month, 1);

        for (var d = start; d <= end; d = unit switch
             {
                 TimeUnit.Hour => d.AddHours(1),
                 TimeUnit.Day => d.AddDays(1),
                 _ => d.AddMonths(1)
             })
        {
            yield return d;
        }
    }
}
