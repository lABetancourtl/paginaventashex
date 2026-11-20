using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;
using PaginaVentasNet.Api.Modules.Home.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Home.Infrastructure;

public class HomeSectionProductRepository : IHomeSectionProductRepository
{
    private readonly AppDbContext _context;

    public HomeSectionProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<HomeSectionProduct>> GetBySectionIdAsync(int sectionId)
    {
        return await _context.HomeSectionProducts
            .Where(p => p.SectionId == sectionId)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();
    }

    public async Task<List<HomeSectionProductResponseDto>> GetDtosBySectionIdAsync(int sectionId)
    {
        var visibleProductIds = _context.Products.WhereVisible().Select(p => p.Id);

        return await _context.HomeSectionProducts
            .Where(hp => hp.SectionId == sectionId)
            .OrderBy(hp => hp.DisplayOrder)
            .Select(hp => new HomeSectionProductResponseDto
            {
                ProductId = hp.ProductId,
                DisplayOrder = hp.DisplayOrder,
                Name = hp.Product!.Name,
                Price = hp.Product.Price,
                DiscountPercent = hp.Product.DiscountPercent,
                CatalogMainUrl = hp.Product.Media
                    .Where(m => m.MediaType == MediaType.Image && m.DisplayOrder == 1)
                    .Select(m => m.Url)
                    .FirstOrDefault(),
                InStock = hp.Product.Variants.Any(v => v.IsActive && v.Stock > 0),
                IsActive = hp.Product.IsActive,
                IsVisible = visibleProductIds.Contains(hp.ProductId)
            })
            .ToListAsync();
    }

    public async Task<List<int>> GetExistingProductIdsAsync(IEnumerable<int> productIds)
    {
        var ids = productIds.Distinct().ToList();

        return await _context.Products
            .Where(p => ids.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();
    }

    public async Task AddAsync(HomeSectionProduct item)
    {
        await _context.HomeSectionProducts.AddAsync(item);
    }

    public Task RemoveAsync(HomeSectionProduct item)
    {
        _context.HomeSectionProducts.Remove(item);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}