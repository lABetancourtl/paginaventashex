using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Home.Application.Dtos;

/// <summary>
/// Un producto de un estante, tal como lo ve el admin.
/// </summary>
public class HomeSectionProductResponseDto
{
    public int ProductId { get; set; }
    public int DisplayOrder { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int? DiscountPercent { get; set; }

    /// <summary>Precio que paga el cliente. Se calcula con la misma fórmula del dominio.</summary>
    public decimal FinalPrice => PriceCalculator.Final(Price, DiscountPercent);

    public string? CatalogMainUrl { get; set; }
    public bool InStock { get; set; }

    /// <summary>Si el producto está activo.</summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Si el producto se ve en la tienda: activo, y su subcategoría y su categoría principal también.
    /// Un producto no visible no sale en la página de inicio.
    /// </summary>
    public bool IsVisible { get; set; }
}