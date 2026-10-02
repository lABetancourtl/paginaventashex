using PaginaVentasNet.Api.Modules.Identity.Application.Otp.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Otp.Ports;
using PaginaVentasNet.Api.Modules.Identity.Domain;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Otp.UseCases;

/// <summary>
/// Caso de uso para generar y enviar un código OTP al email del usuario.
/// Funciona tanto para usuarios nuevos como existentes.
/// </summary>
public class SendOtpUseCase
{
    private readonly IOtpRepository _otpRepository;
    private readonly IEmailService _emailService;

    public SendOtpUseCase(IOtpRepository otpRepository, IEmailService emailService)
    {
        _otpRepository = otpRepository;
        _emailService = emailService;
    }

    public async Task ExecuteAsync(SendOtpDto dto)
    {
        var otpCode = OtpCode.Create(dto.Email);

        await _otpRepository.AddAsync(otpCode);
        await _otpRepository.SaveChangesAsync();

        await _emailService.SendOtpAsync(dto.Email, otpCode.Code);
    }
}