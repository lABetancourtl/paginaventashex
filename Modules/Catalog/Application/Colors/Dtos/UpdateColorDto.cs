using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Dtos;

public class UpdateColorDto
{
    [Required(ErrorMessage = "El nombre del color es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [RegularExpression("^#[0-9A-Fa-f]{6}$",
        ErrorMessage = "El código de color debe tener el formato #RRGGBB, por ejemplo #1E90FF.")]
    public string? HexCode { get; set; }
}