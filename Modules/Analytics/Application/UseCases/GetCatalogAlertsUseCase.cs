using PaginaVentasNet.Api.Modules.Analytics.Application.Dtos;
using PaginaVentasNet.Api.Modules.Analytics.Application.Ports;
using PaginaVentasNet.Api.Modules.Analytics.Domain;

namespace PaginaVentasNet.Api.Modules.Analytics.Application.UseCases;

/// <summary>
/// Cruza las vistas con el catálogo visible:
/// productos sin stock que la gente está viendo (qué reponer) y productos con stock
/// que casi nadie ve (qué destacar).
/// </summary>
public class GetCatalogAlertsUseCase
{
    private const int MaxItems = 10;

    private readonly IAnalyticsProvider _provider;
    private readonly IAnalyticsCatalogLookup _catalog;

    public GetCatalogAlertsUseCase(IAnalyticsProvider provider, IAnalyticsCatalogLookup catalog)
    {
        _provider = provider;
        _catalog = catalog;
    }

    public async Task<CatalogAlertsDto> ExecuteAsync(DateOnly? from, DateOnly? to)
    {
        var range = AnalyticsRange.Create(from, to);
        var views = ProductViewCounter.Count(await _provider.GetPathViewsAsync(range.StartUtc, range.EndUtc));
        var products = await _catalog.GetVisibleProductsAsync();

        CatalogAlertProductDto ToDto(CatalogProduct p) => new()
        {
            ProductId = p.Id,
            Name = p.Name,
            CategoryName = p.CategoryName,
            Stock = p.Stock,
            Views = views.GetValueOrDefault(p.Id)
        };

        return new CatalogAlertsDto
        {
            ViewedOutOfStock = products
                .Where(p => !p.InStock && views.GetValueOrDefault(p.Id) > 0)
                .Select(ToDto)
                .OrderByDescending(p => p.Views)
                .ThenBy(p => p.Name)
                .Take(MaxItems)
                .ToList(),

            LeastViewed = products
                .Where(p => p.InStock)
                .Select(ToDto)
                .OrderBy(p => p.Views)
                .ThenBy(p => p.Name)
                .Take(MaxItems)
                .ToList()
        };
    }
}
