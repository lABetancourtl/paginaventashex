using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;
using PaginaVentasNet.Api.Modules.Home.Domain;

namespace PaginaVentasNet.Api.Modules.Home.Infrastructure;

public class HomeSectionItemRepository : IHomeSectionItemRepository
{
    private readonly AppDbContext _context;

    public HomeSectionItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(HomeSectionItem item)
    {
        await _context.HomeSectionItems.AddAsync(item);
    }

    public async Task<HomeSectionItem?> GetEntityByIdAsync(int id)
    {
        return await _context.HomeSectionItems.FindAsync(id);
    }

    public async Task<List<HomeSectionItem>> GetBySectionIdAsync(int sectionId)
    {
        return await _context.HomeSectionItems
            .Where(i => i.SectionId == sectionId)
            .OrderBy(i => i.DisplayOrder)
            .ThenBy(i => i.Id)
            .ToListAsync();
    }

    public async Task<List<HomeSectionItemResponseDto>> GetDtosBySectionIdAsync(int sectionId)
    {
        return await _context.HomeSectionItems
            .Where(i => i.SectionId == sectionId)
            .OrderBy(i => i.DisplayOrder)
            .ThenBy(i => i.Id)
            .Select(i => new HomeSectionItemResponseDto
            {
                Id = i.Id,
                SectionId = i.SectionId,
                Url = i.Url,
                MediaType = i.MediaType,
                CategoryId = i.CategoryId,
                CategoryName = i.Category == null ? null : i.Category.Name,
                CategorySlug = i.Category == null ? null : i.Category.Slug,
                DisplayOrder = i.DisplayOrder
            })
            .ToListAsync();
    }

    public Task RemoveAsync(HomeSectionItem item)
    {
        _context.HomeSectionItems.Remove(item);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}