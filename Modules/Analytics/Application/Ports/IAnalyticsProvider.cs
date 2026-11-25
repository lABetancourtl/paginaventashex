namespace PaginaVentasNet.Api.Modules.Analytics.Application.Ports;

public enum TimeUnit { Hour, Day, Month }

public record TrafficStats(long Pageviews, long Visitors, long Visits, long Bounces, long TotalTimeSeconds);

/// <summary>Un punto de la serie. Date está en hora de Colombia.</summary>
public record TrafficPoint(DateTime Date, long Pageviews, long Visitors);

public record PathViews(string Path, long Views);

public enum AnalyticsMetric { Device, Country, Region, City }

/// <summary>Visitantes únicos por valor (ej: "mobile", "CO", "Armenia"). Value vacío = desconocido.</summary>
public record MetricCount(string Value, long Visitors);

/// <summary>
/// Fuente de los datos de tráfico (Umami). Las fechas se reciben en UTC.
/// Lanza AppException ANALYTICS_UNAVAILABLE si el servicio no responde.
/// </summary>
public interface IAnalyticsProvider
{
    Task<TrafficStats> GetStatsAsync(DateTime startUtc, DateTime endUtc);
    Task<List<TrafficPoint>> GetSeriesAsync(DateTime startUtc, DateTime endUtc, TimeUnit unit);
    Task<List<PathViews>> GetPathViewsAsync(DateTime startUtc, DateTime endUtc);
    Task<List<MetricCount>> GetMetricAsync(DateTime startUtc, DateTime endUtc, AnalyticsMetric metric, int limit);

    /// <summary>Personas en la tienda en este momento (últimos minutos, según Umami).</summary>
    Task<long> GetActiveVisitorsAsync();
}
