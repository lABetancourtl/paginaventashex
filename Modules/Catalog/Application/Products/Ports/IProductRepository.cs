using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task<bool> ExistsBySkuAsync(string sku);
    Task SaveChangesAsync();
}