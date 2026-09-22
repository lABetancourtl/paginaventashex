using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Products;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public async Task<bool> ExistsBySkuAsync(string sku)
    {
        return await _context.Products
            .AnyAsync(p => p.Sku == sku);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<ProductResponseDto>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)   
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Sku = p.Sku,
                Stock = p.Stock,
                IsActive = p.IsActive,
                CategoryId = p.CategoryId,
                CategoryName = p.Category!.Name,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToListAsync();
    }

    public async Task<List<ProductResponseDto>> GetDeactiveAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive == false)
            .OrderBy(p => p.Name)   
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Sku = p.Sku,
                Stock = p.Stock,
                IsActive = p.IsActive,
                CategoryId = p.CategoryId,
                CategoryName = p.Category!.Name,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToListAsync();
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Id == id)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Sku = p.Sku,
                Stock = p.Stock,
                IsActive = p.IsActive,
                CategoryId = p.CategoryId,
                CategoryName = p.Category!.Name,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Product?> GetEntityByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        return Task.CompletedTask;
    }

    public async Task<PagedResultDto<ProductResponseDto>> SearchAsync(SearchProductsDto dto)
    {
        const int pageSize = 10;

        var query = _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(dto.Search))
        {
            var search = dto.Search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Category!.Name.ToLower().Contains(search));
        }

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((dto.Page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Sku = p.Sku,
                Stock = p.Stock,
                IsActive = p.IsActive,
                CategoryId = p.CategoryId,
                CategoryName = p.Category!.Name,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToListAsync();

        return new PagedResultDto<ProductResponseDto>
        {
            Items = items,
            TotalItems = totalItems,
            Page = dto.Page
        };
    }


}