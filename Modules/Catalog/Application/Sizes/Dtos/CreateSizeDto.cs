using System.ComponentModel.DataAnnotations;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Dtos;

public class CreateSizeDto
{
    [Required(ErrorMessage = "El grupo de la talla es obligatorio.")]
    public SizeGroup? SizeGroup { get; set; }

    [Required(ErrorMessage = "El nombre de la talla es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres.")]
    public string Name { get; set; } = string.Empty;
}