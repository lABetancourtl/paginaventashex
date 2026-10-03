using PaginaVentasNet.Api.Modules.Identity.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Profile.Dtos;

/// <summary>
/// DTO de respuesta con los datos del perfil del usuario autenticado.
/// </summary>
public class ProfileResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Documento { get; set; }
    public Genero? Genero { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public string? Telefono { get; set; }
    public bool TienePassword { get; set; }
    public DateTime CreadoEn { get; set; }
}