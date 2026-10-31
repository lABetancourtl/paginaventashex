using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Variants;

public class SizeRepository : ISizeRepository
{
    private readonly AppDbContext _context;

    public SizeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Size size)
    {
        await _context.Sizes.AddAsync(size);
    }

    public async Task<Size?> GetEntityByIdAsync(int id)
    {
        return await _context.Sizes.FindAsync(id);
    }

    public async Task<List<SizeResponseDto>> GetAllAsync(SizeGroup? sizeGroup, bool includeInactive)
    {
        var query = _context.Sizes.AsQueryable();

        if (sizeGroup.HasValue)
            query = query.Where(s => s.SizeGroup == sizeGroup.Value);

        if (!includeInactive)
            query = query.Where(s => s.IsActive);

        return await query
            .OrderBy(s => s.SizeGroup)
            .ThenBy(s => s.DisplayOrder)
            .Select(s => new SizeResponseDto
            {
                Id = s.Id,
                SizeGroup = s.SizeGroup,
                Name = s.Name,
                DisplayOrder = s.DisplayOrder,
                IsActive = s.IsActive
            })
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(SizeGroup sizeGroup, string name, int? excludeId = null)
    {
        var normalized = name.Trim().ToLower();

        return await _context.Sizes.AnyAsync(s =>
            s.SizeGroup == sizeGroup
            && s.Name.ToLower() == normalized
            && (excludeId == null || s.Id != excludeId));
    }

    public async Task<int> GetNextDisplayOrderAsync(SizeGroup sizeGroup)
    {
        var maxOrder = await _context.Sizes
            .Where(s => s.SizeGroup == sizeGroup)
            .MaxAsync(s => (int?)s.DisplayOrder);

        return (maxOrder ?? 0) + 1;
    }

    public async Task<Size?> GetSiblingByDisplayOrderAsync(SizeGroup sizeGroup, int displayOrder)
    {
        return await _context.Sizes
            .FirstOrDefaultAsync(s => s.SizeGroup == sizeGroup && s.DisplayOrder == displayOrder);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}