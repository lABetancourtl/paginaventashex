using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Media;

/// <summary>
/// Implementación del repositorio de multimedia de productos.
/// </summary>
public class ProductMediaRepository : IProductMediaRepository
{
    private readonly AppDbContext _context;

    public ProductMediaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ProductMedia media)
    {
        await _context.ProductMedia.AddAsync(media);
    }

    public async Task<List<ProductMedia>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductMedia
            .Where(m => m.ProductId == productId)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();
    }

    public async Task<ProductMedia?> GetByIdAsync(int id)
    {
        return await _context.ProductMedia.FindAsync(id);
    }

    public Task RemoveAsync(ProductMedia media)
    {
        _context.ProductMedia.Remove(media);
        return Task.CompletedTask;
    }

    public async Task<int> CountImagesByProductIdAsync(int productId)
    {
        return await _context.ProductMedia
            .CountAsync(m => m.ProductId == productId && m.MediaType == MediaType.Image);
    }

    public async Task<bool> HasVideoAsync(int productId)
    {
        return await _context.ProductMedia
            .AnyAsync(m => m.ProductId == productId && m.MediaType == MediaType.Video);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}