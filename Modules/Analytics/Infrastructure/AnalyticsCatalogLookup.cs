using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Analytics.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Analytics.Infrastructure;

public class AnalyticsCatalogLookup : IAnalyticsCatalogLookup
{
    private readonly AppDbContext _context;

    public AnalyticsCatalogLookup(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductInfo>> GetProductsAsync(IEnumerable<int> productIds)
    {
        var ids = productIds.Distinct().ToList();
        if (ids.Count == 0)
            return new List<ProductInfo>();

        return await _context.Products
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .Select(p => new ProductInfo(p.Id, p.Name, p.CategoryId, p.Category!.Name))
            .ToListAsync();
    }

    public async Task<List<CategoryInfo>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryInfo(c.Id, c.Name, c.Slug, c.ParentCategoryId, c.DisplayOrder))
            .ToListAsync();
    }

    public async Task<List<CatalogProduct>> GetVisibleProductsAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .WhereVisible()
            .Select(p => new CatalogProduct(
                p.Id,
                p.Name,
                p.Category!.Name,
                p.Variants.Any(v => v.IsActive && v.Stock > 0),
                p.Variants.Where(v => v.IsActive).Sum(v => v.Stock)))
            .ToListAsync();
    }
}
