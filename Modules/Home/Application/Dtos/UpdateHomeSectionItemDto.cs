using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Home.Application.Dtos;

/// <summary>
/// Cambia el enlace de un medio. Reemplaza el enlace: si no se envía categoryId,
/// el medio deja de ser un enlace.
/// </summary>
public class UpdateHomeSectionItemDto
{
    [Range(1, int.MaxValue, ErrorMessage = "La categoría no es válida.")]
    public int? CategoryId { get; set; }
}