using PaginaVentasNet.Api.Modules.Identity.Domain;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Otp.Ports;

/// <summary>
/// Puerto de salida para operaciones de persistencia de códigos OTP.
/// </summary>
public interface IOtpRepository
{
    Task AddAsync(OtpCode otpCode);
    Task<OtpCode?> GetValidByEmailAsync(string email, string code);
    Task SaveChangesAsync();
}