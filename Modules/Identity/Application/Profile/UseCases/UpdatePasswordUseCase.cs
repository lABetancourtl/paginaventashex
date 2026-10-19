using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Ports;
using PaginaVentasNet.Api.Modules.Identity.Application.Otp.Ports;
using PaginaVentasNet.Api.Modules.Identity.Application.Profile.Dtos;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Profile.UseCases;

/// <summary>
/// Caso de uso para asignar o cambiar la contraseña del usuario autenticado.
/// Si el usuario ya tiene contraseña, requiere validación con código OTP.
/// Si es la primera vez que asigna contraseña, no requiere OTP.
/// </summary>
public class UpdatePasswordUseCase
{
    private readonly IUsuarioRepository _repository;
    private readonly IOtpRepository _otpRepository;

    public UpdatePasswordUseCase(
        IUsuarioRepository repository,
        IOtpRepository otpRepository)
    {
        _repository = repository;
        _otpRepository = otpRepository;
    }

    public async Task ExecuteAsync(int usuarioId, UpdatePasswordDto dto)
    {
        var usuario = await _repository.GetByIdAsync(usuarioId);

        if (usuario is null)
            throw new InvalidOperationException("Usuario no encontrado.");

        if (string.IsNullOrWhiteSpace(dto.OtpCode))
            throw new ArgumentException(
                "Debes verificar tu identidad con el código enviado a tu correo.");

        var otpCode = await _otpRepository.GetValidByEmailAsync(
            usuario.Email, dto.OtpCode);

        if (otpCode is null || !otpCode.IsValid())
            throw new ArgumentException(
                "El código de verificación es inválido o ha expirado.");

        otpCode.MarkAsUsed();

        var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        usuario.UpdatePassword(newPasswordHash);

        await _repository.UpdateAsync(usuario);
        await _repository.SaveChangesAsync();
    }
}