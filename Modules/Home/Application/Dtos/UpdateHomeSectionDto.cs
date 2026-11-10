using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Home.Application.Dtos;

public class UpdateHomeSectionDto
{
    [StringLength(100, ErrorMessage = "El título no puede superar 100 caracteres.")]
    public string? Title { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La categoría no es válida.")]
    public int? CategoryId { get; set; }
}