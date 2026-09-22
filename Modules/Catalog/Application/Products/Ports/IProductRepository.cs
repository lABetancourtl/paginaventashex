using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task<bool> ExistsBySkuAsync(string sku);
    Task SaveChangesAsync();
    Task<List<ProductResponseDto>> GetAllAsync();
    Task<List<ProductResponseDto>> GetDeactiveAsync();
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task UpdateAsync(Product product);
    Task<Product?> GetEntityByIdAsync(int id);
    Task<PagedResultDto<ProductResponseDto>> SearchAsync(SearchProductsDto dto);
}