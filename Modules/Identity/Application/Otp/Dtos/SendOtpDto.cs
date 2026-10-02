using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Otp.Dtos;

/// <summary>
/// DTO para solicitar el envío de un código OTP.
/// </summary>
public class SendOtpDto
{
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;
}