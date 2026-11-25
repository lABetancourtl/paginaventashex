using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Analytics.Application.Dtos;
using PaginaVentasNet.Api.Modules.Analytics.Application.Ports;
using PaginaVentasNet.Api.Modules.Analytics.Application.UseCases;

namespace PaginaVentasNet.Api.Modules.Analytics.Presentation;

/// <summary>
/// Estadísticas de tráfico de la tienda para el panel admin. Solo Admin.
/// Las fechas son días de Colombia (yyyy-MM-dd). Sin fechas: los últimos 30 días.
/// Si el servicio de estadísticas no responde: 503 ANALYTICS_UNAVAILABLE.
/// </summary>
[Authorize(Roles = "Admin")]
[Route("api/admin/analytics")]
public class AnalyticsController : ApiController
{
    private readonly GetTrafficSummaryUseCase _summaryUseCase;
    private readonly GetTrafficTimeSeriesUseCase _timeSeriesUseCase;
    private readonly GetTrafficByHourUseCase _byHourUseCase;
    private readonly GetProductViewsUseCase _productViewsUseCase;
    private readonly GetCategoryViewsUseCase _categoryViewsUseCase;
    private readonly GetAudienceUseCase _audienceUseCase;
    private readonly GetRealtimeUseCase _realtimeUseCase;
    private readonly GetCatalogAlertsUseCase _catalogAlertsUseCase;

    public AnalyticsController(
        GetTrafficSummaryUseCase summaryUseCase,
        GetTrafficTimeSeriesUseCase timeSeriesUseCase,
        GetTrafficByHourUseCase byHourUseCase,
        GetProductViewsUseCase productViewsUseCase,
        GetCategoryViewsUseCase categoryViewsUseCase,
        GetAudienceUseCase audienceUseCase,
        GetRealtimeUseCase realtimeUseCase,
        GetCatalogAlertsUseCase catalogAlertsUseCase)
    {
        _summaryUseCase = summaryUseCase;
        _timeSeriesUseCase = timeSeriesUseCase;
        _byHourUseCase = byHourUseCase;
        _productViewsUseCase = productViewsUseCase;
        _categoryViewsUseCase = categoryViewsUseCase;
        _audienceUseCase = audienceUseCase;
        _realtimeUseCase = realtimeUseCase;
        _catalogAlertsUseCase = catalogAlertsUseCase;
    }

    /// <summary>
    /// Visitantes, visitas, páginas vistas, duración promedio de la visita (segundos) y tasa de rebote,
    /// con el periodo anterior de la misma longitud para comparar.
    /// </summary>
    [HttpGet("summary")]
    public Task<ActionResult<ApiResponse<TrafficSummaryDto>>> Summary(DateOnly? from, DateOnly? to)
        => Run(() => _summaryUseCase.ExecuteAsync(from, to));

    /// <summary>
    /// Visitantes y páginas vistas por intervalo (Hour, Day o Month). Los intervalos sin visitas vienen en cero.
    /// Por hora, hasta 31 días.
    /// </summary>
    [HttpGet("timeseries")]
    public Task<ActionResult<ApiResponse<List<TimeSeriesPointDto>>>> TimeSeries(
        DateOnly? from, DateOnly? to, TimeUnit unit = TimeUnit.Day)
        => Run(() => _timeSeriesUseCase.ExecuteAsync(from, to, unit));

    /// <summary>Tráfico del rango agrupado en las 24 horas del día (hora de Colombia).</summary>
    [HttpGet("hours")]
    public Task<ActionResult<ApiResponse<List<HourBucketDto>>>> Hours(DateOnly? from, DateOnly? to)
        => Run(() => _byHourUseCase.ExecuteAsync(from, to));

    /// <summary>Productos más vistos, de mayor a menor. limit entre 1 y 100.</summary>
    [HttpGet("products")]
    public Task<ActionResult<ApiResponse<List<ProductViewsDto>>>> Products(
        DateOnly? from, DateOnly? to, int limit = 20)
        => Run(() => _productViewsUseCase.ExecuteAsync(from, to, limit));

    /// <summary>
    /// Árbol de categorías → subcategorías con sus vistas. TotalViews suma la categoría y sus subcategorías;
    /// ProductViews suma las vistas de los productos que pertenecen a ellas.
    /// </summary>
    [HttpGet("categories")]
    public Task<ActionResult<ApiResponse<List<CategoryViewsDto>>>> Categories(DateOnly? from, DateOnly? to)
        => Run(() => _categoryViewsUseCase.ExecuteAsync(from, to));

    /// <summary>
    /// Dispositivos (mobile, computer, tablet), países (código ISO) y ciudades, con su % de visitantes.
    /// La ubicación sale de la IP: las visitas desde la red local no tienen ubicación.
    /// </summary>
    [HttpGet("audience")]
    public Task<ActionResult<ApiResponse<AudienceDto>>> Audience(DateOnly? from, DateOnly? to)
        => Run(() => _audienceUseCase.ExecuteAsync(from, to));

    /// <summary>Personas en la tienda en este momento.</summary>
    [HttpGet("realtime")]
    public Task<ActionResult<ApiResponse<RealtimeDto>>> Realtime()
        => Run(() => _realtimeUseCase.ExecuteAsync());

    /// <summary>
    /// Productos visibles sin stock que se están viendo (reponer) y productos con stock con menos vistas (destacar).
    /// </summary>
    [HttpGet("catalog-alerts")]
    public Task<ActionResult<ApiResponse<CatalogAlertsDto>>> CatalogAlerts(DateOnly? from, DateOnly? to)
        => Run(() => _catalogAlertsUseCase.ExecuteAsync(from, to));

    private async Task<ActionResult<ApiResponse<T>>> Run<T>(Func<Task<T>> action)
    {
        try
        {
            return Success(await action());
        }
        catch (AppException ex)
        {
            return Failure<T>(ex.Code, ex.Message, ex.StatusCode);
        }
    }
}
