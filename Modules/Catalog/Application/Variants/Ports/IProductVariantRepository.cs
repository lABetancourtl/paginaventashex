using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Ports;

public interface IProductVariantRepository
{
    Task AddAsync(ProductVariant variant);
    Task<ProductVariant?> GetEntityByIdAsync(int id);
    Task<List<ProductVariant>> GetByProductIdAsync(int productId);
    Task<List<ProductVariantResponseDto>> GetAllDtosByProductIdAsync(int productId);
    Task<bool> ExistsBySkuAsync(string sku, int? excludeId = null);
    Task SaveChangesAsync();
}