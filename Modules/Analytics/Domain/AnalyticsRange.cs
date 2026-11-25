using PaginaVentasNet.Api.Common.Exceptions;

namespace PaginaVentasNet.Api.Modules.Analytics.Domain;

/// <summary>
/// Rango de fechas de una consulta de tráfico. Las fechas son días de Colombia:
/// el rango va desde el inicio del día "from" hasta el final del día "to".
/// </summary>
public sealed class AnalyticsRange
{
    public const int DefaultDays = 30;
    public const int MaxDays = 366;
    public const string TimeZoneId = "America/Bogota";

    public static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);

    public DateOnly From { get; }
    public DateOnly To { get; }
    public DateTime StartUtc { get; }
    public DateTime EndUtc { get; }

    private AnalyticsRange(DateOnly from, DateOnly to)
    {
        From = from;
        To = to;
        StartUtc = TimeZoneInfo.ConvertTimeToUtc(
            from.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified), TimeZone);
        EndUtc = TimeZoneInfo.ConvertTimeToUtc(
            to.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Unspecified), TimeZone);
    }

    /// <summary>Sin fechas: los últimos 30 días, incluido hoy.</summary>
    public static AnalyticsRange Create(DateOnly? from, DateOnly? to)
    {
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone));
        var end = to ?? today;
        var start = from ?? end.AddDays(-(DefaultDays - 1));

        if (start > end)
            throw new AppException("ANALYTICS_INVALID_RANGE",
                "La fecha inicial no puede ser posterior a la fecha final.", 400);

        if (end.DayNumber - start.DayNumber + 1 > MaxDays)
            throw new AppException("ANALYTICS_INVALID_RANGE",
                $"El rango no puede superar {MaxDays} días.", 400);

        return new AnalyticsRange(start, end);
    }

    /// <summary>Periodo inmediatamente anterior, con la misma cantidad de días.</summary>
    public AnalyticsRange Previous()
    {
        var days = To.DayNumber - From.DayNumber + 1;
        return new AnalyticsRange(From.AddDays(-days), From.AddDays(-1));
    }
}
