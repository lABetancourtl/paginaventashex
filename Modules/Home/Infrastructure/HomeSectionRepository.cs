using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;
using PaginaVentasNet.Api.Modules.Home.Domain;

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