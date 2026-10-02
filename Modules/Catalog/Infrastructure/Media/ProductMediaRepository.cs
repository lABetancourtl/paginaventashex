using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;
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

    /// <summary>
    /// Obtiene todos los archivos multimedia asociados a un producto dado su ID. 
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    public async Task<List<ProductMedia>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductMedia
            .Where(m => m.ProductId == productId)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un archivo multimedia por su ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    public async Task<List<ProductMediaResponseDto>> GetAllByProductIdAsync(int productId)
    {
        return await _context.ProductMedia
            .Where(m => m.ProductId == productId)
            .OrderBy(m => m.DisplayOrder)
            .Select(m => new ProductMediaResponseDto
            {
                Id = m.Id,
                Url = m.Url,
                MediaType = m.MediaType,
                DisplayOrder = m.DisplayOrder,
                IsMain = m.IsMain
            })
            .ToListAsync();
    }

    public async Task<ProductMedia?> GetNextImageAsync(int productId, int excludeMediaId)
    {
        return await _context.ProductMedia
            .Where(m => m.ProductId == productId 
                    && m.Id != excludeMediaId 
                    && m.MediaType == MediaType.Image)
            .OrderBy(m => m.DisplayOrder)
            .FirstOrDefaultAsync();
    }

    public async Task<ProductMedia?> GetIsMainAsync(int productId)
    {
        return await _context.ProductMedia
            .Where(m => m.ProductId == productId && m.IsMain)
            .FirstOrDefaultAsync();
    }

}