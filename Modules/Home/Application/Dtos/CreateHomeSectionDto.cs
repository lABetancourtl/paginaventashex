using System.ComponentModel.DataAnnotations;
using PaginaVentasNet.Api.Modules.Home.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Home.Application.Dtos;

public class CreateHomeSectionDto
{
    [Required(ErrorMessage = "El tipo de sección es obligatorio.")]
    public HomeSectionType? Type { get; set; }

    [StringLength(100, ErrorMessage = "El título no puede superar 100 caracteres.")]
    public string? Title { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La categoría no es válida.")]
    public int? CategoryId { get; set; }
}