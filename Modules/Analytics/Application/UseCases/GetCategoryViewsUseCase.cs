using PaginaVentasNet.Api.Modules.Analytics.Application.Dtos;
using PaginaVentasNet.Api.Modules.Analytics.Application.Ports;
using PaginaVentasNet.Api.Modules.Analytics.Domain;

namespace PaginaVentasNet.Api.Modules.Analytics.Application.UseCases;

/// <summary>
/// Vistas por categoría y subcategoría, en árbol. Cada categoría trae las vistas de su página,
/// el total con sus subcategorías y las vistas de sus productos.
/// Las vistas de categorías que ya no existen se agrupan al final.
/// </summary>
public class GetCategoryViewsUseCase
{
    private const string UnknownName = "Categorías eliminadas o desconocidas";

    private readonly IAnalyticsProvider _provider;
    private readonly IAnalyticsCatalogLookup _catalog;

    public GetCategoryViewsUseCase(IAnalyticsProvider provider, IAnalyticsCatalogLookup catalog)
    {
        _provider = provider;
        _catalog = catalog;
    }

    public async Task<List<CategoryViewsDto>> ExecuteAsync(DateOnly? from, DateOnly? to)
    {
        var range = AnalyticsRange.Create(from, to);
        var paths = await _provider.GetPathViewsAsync(range.StartUtc, range.EndUtc);
        var categories = await _catalog.GetAllCategoriesAsync();

        // Vistas de las páginas de categoría, por slug
        var viewsBySlug = new Dictionary<string, long>();
        foreach (var path in paths)
        {
            if (StorePath.TryGetCategorySlug(path.Path, out var slug))
                viewsBySlug[slug] = viewsBySlug.GetValueOrDefault(slug) + path.Views;
        }

        // Vistas de productos, por la categoría a la que pertenece cada producto
        var productViews = ProductViewCounter.Count(paths);
        var products = await _catalog.GetProductsAsync(productViews.Keys);
        var productViewsByCategory = products
            .GroupBy(p => p.CategoryId)
            .ToDictionary(g => g.Key, g => g.Sum(p => productViews[p.Id]));

        var bySlug = categories.ToDictionary(c => c.Slug.ToLowerInvariant());
        var childrenOf = categories
            .Where(c => c.ParentCategoryId is not null)
            .GroupBy(c => c.ParentCategoryId!.Value)
            .ToDictionary(g => g.Key, g => g.OrderBy(c => c.DisplayOrder).ToList());

        CategoryViewsDto Build(CategoryInfo c)
        {
            var subcategories = childrenOf.GetValueOrDefault(c.Id, new List<CategoryInfo>())
                .Select(Build)
                .OrderByDescending(s => s.TotalViews)
                .ToList();

            var views = viewsBySlug.GetValueOrDefault(c.Slug.ToLowerInvariant());

            return new CategoryViewsDto
            {
                CategoryId = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Views = views,
                TotalViews = views + subcategories.Sum(s => s.TotalViews),
                ProductViews = productViewsByCategory.GetValueOrDefault(c.Id)
                               + subcategories.Sum(s => s.ProductViews),
                Subcategories = subcategories
            };
        }

        var result = categories
            .Where(c => c.ParentCategoryId is null)
            .Select(Build)
            .OrderByDescending(c => c.TotalViews)
            .ThenByDescending(c => c.ProductViews)
            .ToList();

        var unknownViews = viewsBySlug
            .Where(kv => !bySlug.ContainsKey(kv.Key))
            .Sum(kv => kv.Value);

        if (unknownViews > 0)
        {
            result.Add(new CategoryViewsDto
            {
                CategoryId = null,
                Name = UnknownName,
                Views = unknownViews,
                TotalViews = unknownViews
            });
        }

        return result;
    }
}
