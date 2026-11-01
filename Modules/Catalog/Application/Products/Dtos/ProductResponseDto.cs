using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Domain;
using System.Text.Json.Serialization;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Dtos;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
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

    /// <summary>Variantes del producto. Solo se llena en el detalle; en las listas va vacía.</summary>
    public List<ProductVariantResponseDto> Variants { get; set; } = new();

    /// <summary>true si alguna variante activa tiene stock. Sirve para mostrar "Agotado".</summary>
    public bool InStock { get; set; }

    /// <summary>Suma del stock de las variantes activas. Solo la llena el panel admin.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TotalStock { get; set; }
}