using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Profile.Dtos;

/// <summary>
/// DTO para actualizar los datos del perfil del usuario.
/// </summary>
public class UpdateProfileDto
{
    [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    public string? Nombre { get; set; }

    [StringLength(100, ErrorMessage = "El apellido no puede superar 100 caracteres.")]
    public string? Apellido { get; set; }

    [StringLength(20, ErrorMessage = "El documento no puede superar 20 caracteres.")]
    public string? Documento { get; set; }

    [StringLength(20, ErrorMessage = "El género no puede superar 20 caracteres.")]
    public string? Genero { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono no puede superar 20 caracteres.")]
    public string? Telefono { get; set; }
}