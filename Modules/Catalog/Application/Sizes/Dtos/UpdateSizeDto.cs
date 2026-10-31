using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Dtos;

public class UpdateSizeDto
{
    [Required(ErrorMessage = "El nombre de la talla es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres.")]
    public string Name { get; set; } = string.Empty;
}