using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Variants;

public class ColorRepository : IColorRepository
{
    private readonly AppDbContext _context;

    public ColorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Color color)
    {
        await _context.Colors.AddAsync(color);
    }

    public async Task<Color?> GetEntityByIdAsync(int id)
    {
        return await _context.Colors.FindAsync(id);
    }

    public async Task<List<ColorResponseDto>> GetAllAsync(bool includeInactive)
    {
        var query = _context.Colors.AsQueryable();

        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new ColorResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                HexCode = c.HexCode,
                IsActive = c.IsActive
            })
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var normalized = name.Trim().ToLower();

        return await _context.Colors.AnyAsync(c =>
            c.Name.ToLower() == normalized
            && (excludeId == null || c.Id != excludeId));
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}