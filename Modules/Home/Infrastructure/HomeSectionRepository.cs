using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;
using PaginaVentasNet.Api.Modules.Home.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;
using PaginaVentasNet.Api.Modules.Home.Domain.Enums;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Home.Infrastructure;

public class HomeSectionRepository : IHomeSectionRepository
{
    private readonly AppDbContext _context;

    public HomeSectionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(HomeSection section)
    {
        await _context.HomeSections.AddAsync(section);
    }

    public async Task<HomeSection?> GetEntityByIdAsync(int id)
    {
        return await _context.HomeSections.FindAsync(id);
    }

    public async Task<List<HomeSectionResponseDto>> GetAllDtosAsync()
    {
        return await _context.HomeSections
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Id)
            .Select(s => new HomeSectionResponseDto
            {
                Id = s.Id,
                Type = s.Type,
                Title = s.Title,
                CategoryId = s.CategoryId,
                CategoryName = s.Category == null ? null : s.Category.Name,
                CategorySlug = s.Category == null ? null : s.Category.Slug,
                DisplayOrder = s.DisplayOrder,
                IsActive = s.IsActive,
                ItemCount = s.Items.Count,
                ProductCount = s.Products.Count,
                CreatedAtUtc = s.CreatedAtUtc
            })
            .ToListAsync();
    }

    public async Task<List<HomePageSectionDto>> GetPublicSectionsAsync()
    {
        // La regla de visibilidad vive en CatalogVisibility. Aquí solo se pregunta por pertenencia.
        var visibleCategoryIds = _context.Categories.WhereVisible().Select(c => c.Id);
        var visibleProductIds = _context.Products.WhereVisible().Select(p => p.Id);

        var sections = await _context.HomeSections
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Id)
            .Select(s => new HomePageSectionDto
            {
                Id = s.Id,
                Type = s.Type,
                Title = s.Title,
                ViewAllCategorySlug = s.CategoryId != null && visibleCategoryIds.Contains(s.CategoryId.Value)
                    ? s.Category!.Slug : null,
                ViewAllCategoryName = s.CategoryId != null && visibleCategoryIds.Contains(s.CategoryId.Value)
                    ? s.Category!.Name : null,
                Items = s.Items
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new HomePageItemDto
                    {
                        Id = i.Id,
                        Url = i.Url,
                        MediaType = i.MediaType,
                        // Con varios medios el banner es un carrusel y no usa versión para celular
                        MobileUrl = s.Items.Count == 1 ? i.MobileUrl : null,
                        CategorySlug = i.CategoryId != null && visibleCategoryIds.Contains(i.CategoryId.Value)
                            ? i.Category!.Slug : null,
                        CategoryName = i.CategoryId != null && visibleCategoryIds.Contains(i.CategoryId.Value)
                            ? i.Category!.Name : null
                    })
                    .ToList(),
                Products = s.Products
                    .Where(hp => visibleProductIds.Contains(hp.ProductId))
                    .OrderBy(hp => hp.DisplayOrder)
                    .Select(hp => new HomePageProductDto
                    {
                        Id = hp.Product!.Id,
                        Name = hp.Product.Name,
                        CategoryName = hp.Product.Category!.Name,
                        Price = hp.Product.Price,
                        DiscountPercent = hp.Product.DiscountPercent,
                        CatalogMainUrl = hp.Product.Media
                            .Where(m => m.MediaType == MediaType.Image && m.DisplayOrder == 1)
                            .Select(m => m.Url)
                            .FirstOrDefault(),
                        CatalogHoverUrl = hp.Product.Media
                            .Where(m => m.MediaType == MediaType.Image && m.DisplayOrder == 2)
                            .Select(m => m.Url)
                            .FirstOrDefault(),
                        InStock = hp.Product.Variants.Any(v => v.IsActive && v.Stock > 0)
                    })
                    .ToList()
            })
            .ToListAsync();

        // Una sección que se queda sin nada que mostrar no sale en la página
        return sections
            .Where(s => s.Type == HomeSectionType.Banner || s.Title == HomeSection.IntroTitle
                ? s.Items.Count > 0
                : s.Products.Count > 0)
            .ToList();
    }

    public async Task<int> GetNextDisplayOrderAsync()
    {
        var maxOrder = await _context.HomeSections
            .MaxAsync(s => (int?)s.DisplayOrder);

        return (maxOrder ?? 0) + 1;
    }

    public async Task<HomeSection?> GetByDisplayOrderAsync(int displayOrder)
    {
        return await _context.HomeSections
            .FirstOrDefaultAsync(s => s.DisplayOrder == displayOrder);
    }

    public async Task<List<HomeSection>> GetWithOrderGreaterThanAsync(int displayOrder)
    {
        return await _context.HomeSections
            .Where(s => s.DisplayOrder > displayOrder)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    public Task RemoveAsync(HomeSection section)
    {
        _context.HomeSections.Remove(section);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}