using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Profile.Dtos;

/// <summary>
/// DTO para asignar o cambiar la contraseña del usuario.
/// </summary>
public class UpdatePasswordDto
{
    public string? OtpCode { get; set; }

    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener mínimo 6 caracteres.")]
    public string NewPassword { get; set; } = string.Empty;
}