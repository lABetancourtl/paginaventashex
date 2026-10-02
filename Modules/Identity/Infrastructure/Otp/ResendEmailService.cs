using Resend;
using PaginaVentasNet.Api.Modules.Identity.Application.Otp.Ports;

namespace PaginaVentasNet.Api.Modules.Identity.Infrastructure.Otp;

/// <summary>
/// Adaptador de salida para el servicio de envío de emails usando Resend.
/// </summary>
public class ResendEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly string _fromEmail;
    private readonly string _toEmail;

    public ResendEmailService(IResend resend)
    {
        _resend = resend;
        _fromEmail = Environment.GetEnvironmentVariable("RESEND_FROM_EMAIL")!;
        _toEmail = Environment.GetEnvironmentVariable("RESEND_TO_EMAIL")!;
    }

    public async Task SendOtpAsync(string toEmail, string code)
    {
        var message = new EmailMessage
        {
            From = _fromEmail,
            To = { _toEmail },
            Subject = "Tu código de acceso - PaginaVentasNet",
            HtmlBody = $"""
                <div style="font-family: Arial, sans-serif; max-width: 400px; margin: 0 auto;">
                    <h2>Tu código de acceso</h2>
                    <p>Usa el siguiente código para acceder a tu cuenta:</p>
                    <div style="background: #f4f4f4; padding: 20px; text-align: center; border-radius: 8px;">
                        <h1 style="letter-spacing: 8px; color: #333;">{code}</h1>
                    </div>
                    <p>Este código expira en <strong>10 minutos</strong>.</p>
                    <p>Si no solicitaste este código, ignora este mensaje.</p>
                </div>
                """
        };

        await _resend.EmailSendAsync(message);
    }
}