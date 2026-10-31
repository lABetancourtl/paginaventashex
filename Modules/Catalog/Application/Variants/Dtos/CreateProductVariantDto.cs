using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Dtos;

public class CreateProductVariantDto
{
    [Range(1, int.MaxValue, ErrorMessage = "La talla no es válida.")]
    public int? SizeId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El color no es válido.")]
    public int? ColorId { get; set; }

    [Required(ErrorMessage = "El SKU es obligatorio.")]
    [StringLength(50, ErrorMessage = "El SKU no puede superar 50 caracteres.")]
    public string Sku { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }
}