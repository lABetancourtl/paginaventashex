using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;

/// <summary>
/// Puerto de salida para operaciones de persistencia de multimedia de productos.
/// </summary>
public interface IProductMediaRepository
{
    Task AddAsync(Domain.ProductMedia media);
    Task<List<Domain.ProductMedia>> GetByProductIdAsync(int productId);
    Task<List<ProductMediaResponseDto>> GetAllByProductIdAsync(int productId);
    Task<ProductMedia?> GetByIdAsync(int id);
    Task RemoveAsync(ProductMedia media);
    Task<int> CountImagesByProductIdAsync(int productId);
    Task<bool> HasVideoAsync(int productId);
    Task SaveChangesAsync();
    Task<ProductMedia?> GetNextImageAsync(int productId, int excludeMediaId);
}