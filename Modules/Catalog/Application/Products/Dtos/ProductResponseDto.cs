using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int Stock { get; set; }
    public bool IsActive { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? CatalogMainUrl { get; set; }
    public string? CatalogHoverUrl { get; set; } 
    public List<ProductMediaResponseDto> Media { get; set; } = new();
    public DateTime CreatedAtUtc { get; set; }
    public int? DiscountPercent { get; set; }

    /// <summary>Precio que paga el cliente. Se calcula con la misma fórmula del dominio.</summary>
    public decimal FinalPrice => PriceCalculator.Final(Price, DiscountPercent);
}