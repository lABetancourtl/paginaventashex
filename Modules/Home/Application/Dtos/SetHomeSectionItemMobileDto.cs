using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PaginaVentasNet.Api.Modules.Home.Application.Dtos;

/// <summary>
/// Imagen para celular de un medio de banner. Se envía como multipart/form-data.
/// </summary>
public class SetHomeSectionItemMobileDto
{
    [Required(ErrorMessage = "El archivo es obligatorio.")]
    public IFormFile File { get; set; } = null!;
}
