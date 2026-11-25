using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Analytics.Application.Dtos;
using PaginaVentasNet.Api.Modules.Analytics.Application.Ports;
using PaginaVentasNet.Api.Modules.Analytics.Domain;

namespace PaginaVentasNet.Api.Modules.Analytics.Application.UseCases;

/// <summary>Productos más vistos del periodo, a partir de las visitas a /producto/{id}.</summary>
public class GetProductViewsUseCase
{
    public const int MaxLimit = 100;

    private readonly IAnalyticsProvider _provider;
    private readonly IAnalyticsCatalogLookup _catalog;

    public GetProductViewsUseCase(IAnalyticsProvider provider, IAnalyticsCatalogLookup catalog)
    {
        _provider = provider;
        _catalog = catalog;
    }

    public async Task<List<ProductViewsDto>> ExecuteAsync(DateOnly? from, DateOnly? to, int limit)
    {
        if (limit < 1 || limit > MaxLimit)
            throw new AppException("ANALYTICS_INVALID_LIMIT",
                $"El límite debe estar entre 1 y {MaxLimit}.", 400);

        var range = AnalyticsRange.Create(from, to);
        var paths = await _provider.GetPathViewsAsync(range.StartUtc, range.EndUtc);

        var top = ProductViewCounter.Count(paths)
            .OrderByDescending(kv => kv.Value)
            .ThenBy(kv => kv.Key)
            .Take(limit)
            .ToList();

        var products = (await _catalog.GetProductsAsync(top.Select(kv => kv.Key)))
            .ToDictionary(p => p.Id);

        return top.Select(kv =>
        {
            products.TryGetValue(kv.Key, out var product);
            return new ProductViewsDto
            {
                ProductId = kv.Key,
                Name = product?.Name,
                CategoryId = product?.CategoryId,
                CategoryName = product?.CategoryName,
                Exists = product is not null,
                Views = kv.Value
            };
        }).ToList();
    }
}

/// <summary>Suma las vistas por producto. Varias rutas pueden ser del mismo producto (p. ej. con query string).</summary>
public static class ProductViewCounter
{
    public static Dictionary<int, long> Count(IEnumerable<PathViews> paths)
    {
        var views = new Dictionary<int, long>();

        foreach (var path in paths)
        {
            if (StorePath.TryGetProductId(path.Path, out var id))
                views[id] = views.GetValueOrDefault(id) + path.Views;
        }

        return views;
    }
}
