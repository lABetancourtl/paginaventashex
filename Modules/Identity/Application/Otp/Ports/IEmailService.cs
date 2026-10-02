namespace PaginaVentasNet.Api.Modules.Identity.Application.Otp.Ports;

/// <summary>
/// Puerto de salida para el servicio de envío de emails.
/// </summary>
public interface IEmailService
{
    Task SendOtpAsync(string toEmail, string code);
}