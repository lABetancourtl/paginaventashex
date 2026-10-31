using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Dtos;

public class ProductVariantResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int? SizeId { get; set; }
    public SizeGroup? SizeGroup { get; set; }
    public string? SizeName { get; set; }
    public int? ColorId { get; set; }
    public string? ColorName { get; set; }
    public string? ColorHexCode { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int Stock { get; set; }
    public bool IsActive { get; set; }
}