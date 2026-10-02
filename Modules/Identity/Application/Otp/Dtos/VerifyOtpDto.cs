using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Otp.Dtos;

/// <summary>
/// DTO para verificar un código OTP e iniciar sesión.
/// </summary>
public class VerifyOtpDto
{
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "El código debe tener 6 dígitos.")]
    public string Code { get; set; } = string.Empty;
}