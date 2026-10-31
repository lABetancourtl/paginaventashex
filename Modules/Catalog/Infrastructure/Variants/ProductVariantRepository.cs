using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Variants;

public class ProductVariantRepository : IProductVariantRepository
{
    private readonly AppDbContext _context;

    public ProductVariantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ProductVariant variant)
    {
        await _context.ProductVariants.AddAsync(variant);
    }

    public async Task<ProductVariant?> GetEntityByIdAsync(int id)
    {
        return await _context.ProductVariants.FindAsync(id);
    }

    public async Task<List<ProductVariant>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductVariants
            .Where(v => v.ProductId == productId)
            .ToListAsync();
    }

    public async Task<List<ProductVariantResponseDto>> GetAllDtosByProductIdAsync(int productId)
    {
        return await _context.ProductVariants
            .Where(v => v.ProductId == productId)
            .OrderBy(v => v.Size!.SizeGroup)
            .ThenBy(v => v.Size!.DisplayOrder)
            .ThenBy(v => v.Color!.Name)
            .ThenBy(v => v.Id)
            .Select(v => new ProductVariantResponseDto
            {
                Id = v.Id,
                ProductId = v.ProductId,
                SizeId = v.SizeId,
                SizeGroup = v.Size == null ? (SizeGroup?)null : v.Size.SizeGroup,
                SizeName = v.Size == null ? null : v.Size.Name,
                ColorId = v.ColorId,
                ColorName = v.Color == null ? null : v.Color.Name,
                ColorHexCode = v.Color == null ? null : v.Color.HexCode,
                Sku = v.Sku,
                Stock = v.Stock,
                IsActive = v.IsActive
            })
            .ToListAsync();
    }

    public async Task<bool> ExistsBySkuAsync(string sku, int? excludeId = null)
    {
        var normalized = sku.Trim().ToUpper();

        return await _context.ProductVariants.AnyAsync(v =>
            v.Sku == normalized
            && (excludeId == null || v.Id != excludeId));
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}