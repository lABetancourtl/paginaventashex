namespace PaginaVentasNet.Api.Modules.Analytics.Application.Dtos;

/// <summary>Totales de un periodo.</summary>
public class TrafficTotalsDto
{
    /// <summary>Personas distintas.</summary>
    public long Visitors { get; set; }

    /// <summary>Visitas (sesiones). Una persona puede visitar varias veces.</summary>
    public long Visits { get; set; }

    public long Pageviews { get; set; }

    /// <summary>Duración promedio de una visita, en segundos.</summary>
    public double AvgVisitDurationSeconds { get; set; }

    /// <summary>Porcentaje de visitas que vieron una sola página (0-100).</summary>
    public double BounceRate { get; set; }

    public double PagesPerVisit { get; set; }
}

public class TrafficSummaryDto
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public TrafficTotalsDto Current { get; set; } = new();

    /// <summary>El periodo anterior con la misma cantidad de días, para comparar.</summary>
    public TrafficTotalsDto Previous { get; set; } = new();
}

public class TimeSeriesPointDto
{
    /// <summary>Inicio del intervalo, en hora de Colombia.</summary>
    public DateTime Date { get; set; }
    public long Visitors { get; set; }
    public long Pageviews { get; set; }
}

public class HourBucketDto
{
    /// <summary>Hora del día en Colombia (0-23).</summary>
    public int Hour { get; set; }
    public long Visitors { get; set; }
    public long Pageviews { get; set; }
}

public class ProductViewsDto
{
    public int ProductId { get; set; }

    /// <summary>Nulo si el producto ya no existe.</summary>
    public string? Name { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool Exists { get; set; }
    public long Views { get; set; }
}

public class CategoryViewsDto
{
    /// <summary>Nulo para las vistas de categorías que ya no existen.</summary>
    public int? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }

    /// <summary>Vistas de la página de esta categoría.</summary>
    public long Views { get; set; }

    /// <summary>Vistas de esta categoría más las de sus subcategorías.</summary>
    public long TotalViews { get; set; }

    /// <summary>Vistas de los productos de esta categoría (y de sus subcategorías).</summary>
    public long ProductViews { get; set; }

    public List<CategoryViewsDto> Subcategories { get; set; } = new();
}

/// <summary>
/// Un valor de audiencia. Value es la clave cruda: "mobile" | "computer" | "tablet" para dispositivos,
/// código ISO para países (ej: "CO") y nombre para ciudades. Vacío = desconocido.
/// </summary>
public class AudienceItemDto
{
    public string Value { get; set; } = string.Empty;
    public long Visitors { get; set; }

    /// <summary>Porcentaje sobre el total de visitantes del periodo (0-100).</summary>
    public double Percent { get; set; }
}

public class AudienceDto
{
    public long TotalVisitors { get; set; }
    public List<AudienceItemDto> Devices { get; set; } = new();
    public List<AudienceItemDto> Countries { get; set; } = new();
    public List<AudienceItemDto> Cities { get; set; } = new();
}

public class RealtimeDto
{
    public long ActiveVisitors { get; set; }
}

public class CatalogAlertProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Stock { get; set; }
    public long Views { get; set; }
}

public class CatalogAlertsDto
{
    /// <summary>Productos visibles sin stock que la gente está viendo: ventas perdidas.</summary>
    public List<CatalogAlertProductDto> ViewedOutOfStock { get; set; } = new();

    /// <summary>Productos visibles con stock y pocas vistas (incluye los de cero): candidatos a destacar.</summary>
    public List<CatalogAlertProductDto> LeastViewed { get; set; } = new();
}
