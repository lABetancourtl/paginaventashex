using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PaginaVentasNet.Api.Modules.Home.Application.Dtos;

/// <summary>
/// Datos para agregar un medio a un banner. Se envía como multipart/form-data.
/// </summary>
public class AddHomeSectionItemDto
{
    [Required(ErrorMessage = "El archivo es obligatorio.")]
    public IFormFile File { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "La categoría no es válida.")]
    public int? CategoryId { get; set; }
}