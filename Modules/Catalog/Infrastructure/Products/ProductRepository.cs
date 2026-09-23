using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Products;

/// <summary> 
/// Implementación del repositorioc (interface IProductRepository) de productos.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Agrega un producto al repositorio.
    /// </summary>
    /// <param name="product">Producto a agregar.</param>
    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    /// <summary>
    /// Verifica si existe un producto con el SKU especificado.
    /// </summary>
    /// <param name="sku">SKU del producto a verificar.</param>
    /// <returns>True si el producto existe, false en caso contrario.</returns>
    public async Task<bool> ExistsBySkuAsync(string sku)
    {
        return await _context.Products
            .AnyAsync(p => p.Sku == sku);
    }

    /// <summary>
    /// Guarda los cambios realizados en el contexto de la base de datos.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    /// <summary> 
    /// Obtiene todos los productos activos.
    /// </summary>
    /// <returns>Lista de productos.</returns>
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

    /// <summary>
    /// Obtiene todos los productos inactivos.
    /// </summary>
    /// <returns>Lista de productos.</returns>
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

    /// <summary>
    /// Obtiene un producto por su Id.
    /// </summary>
    /// <param name="id">Id del producto a buscar.</param>
    /// <returns>Datos del producto o null si no existe.</returns>
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

    /// <summary>
    /// Obtiene una entidad de producto por su Id.
    /// </summary>
    /// <param name="id">Id del producto a buscar.</param>
    /// <returns>Entidad del producto o null si no existe.</returns>
    public async Task<Product?> GetEntityByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    /// <summary>
    /// Actualiza un producto.
    /// </summary>
    /// <param name="product">Producto a actualizar.</param>
    /// <returns>Tarea que representa la operación de actualización.</returns>
    public Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Busca productos según los criterios especificados en el DTO de búsqueda.
    /// </summary>
    /// <param name="dto">DTO que contiene los criterios de búsqueda: search, page.</param>
    /// <returns>Resultado paginado de productos que coinciden con los criterios de búsqueda.</returns>
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