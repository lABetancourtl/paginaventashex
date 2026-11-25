namespace PaginaVentasNet.Api.Modules.Analytics.Application.Ports;

public record ProductInfo(int Id, string Name, int CategoryId, string CategoryName);

/// <summary>Producto visible en la tienda con su stock (suma de variantes activas).</summary>
public record CatalogProduct(int Id, string Name, string CategoryName, bool InStock, int Stock);

public record CategoryInfo(int Id, string Name, string Slug, int? ParentCategoryId, int DisplayOrder);

/// <summary>Nombres del catálogo para mostrar las estadísticas con nombres y no con rutas.</summary>
public interface IAnalyticsCatalogLookup
{
    Task<List<ProductInfo>> GetProductsAsync(IEnumerable<int> productIds);
    Task<List<CategoryInfo>> GetAllCategoriesAsync();

    /// <summary>Productos que la tienda muestra al público, con su stock.</summary>
    Task<List<CatalogProduct>> GetVisibleProductsAsync();
}
