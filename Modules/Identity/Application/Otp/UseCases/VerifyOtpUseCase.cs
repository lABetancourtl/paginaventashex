using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Ports;
using PaginaVentasNet.Api.Modules.Identity.Application.Otp.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Otp.Ports;
using PaginaVentasNet.Api.Modules.Identity.Domain;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Otp.UseCases;

/// <summary>
/// Caso de uso para verificar un código OTP.
/// Si el usuario no existe lo crea automáticamente.
/// Retorna un JWT válido.
/// </summary>
public class VerifyOtpUseCase
{
    private readonly IOtpRepository _otpRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtService _jwtService;

    public VerifyOtpUseCase(
        IOtpRepository otpRepository,
        IUsuarioRepository usuarioRepository,
        IJwtService jwtService)
    {
        _otpRepository = otpRepository;
        _usuarioRepository = usuarioRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> ExecuteAsync(VerifyOtpDto dto)
    {
        var otpCode = await _otpRepository.GetValidByEmailAsync(dto.Email, dto.Code);

        if (otpCode is null || !otpCode.IsValid())
            throw new UnauthorizedAccessException(
                "El código es inválido o ha expirado.");

        otpCode.MarkAsUsed();

        var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);

        if (usuario is null)
        {
            usuario = Usuario.Create(dto.Email, string.Empty);
            await _usuarioRepository.AddAsync(usuario);
        }

        await _otpRepository.SaveChangesAsync();

        var token = _jwtService.GenerarToken(usuario);
        var expiracion = int.Parse(
            Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES") ?? "60");

        return new AuthResponseDto
        {
            Token = token,
            Email = usuario.Email,
            Rol = usuario.Rol,
            Expiracion = DateTime.UtcNow.AddMinutes(expiracion)
        };
    }
}