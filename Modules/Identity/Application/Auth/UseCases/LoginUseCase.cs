using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Ports;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Auth.UseCases;

public class LoginUseCase
{
    private readonly IUsuarioRepository _repository;
    private readonly IJwtService _jwtService;

    public LoginUseCase(IUsuarioRepository repository, IJwtService jwtService)
    {
        _repository = repository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> ExecuteAsync(LoginDto dto)
    {
        var usuario = await _repository.GetByEmailAsync(dto.Email);

        if (usuario is null)
            throw new UnauthorizedAccessException("Email o contraseña incorrectos.");

        var passwordValido = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);

        if (!passwordValido)
            throw new UnauthorizedAccessException("Email o contraseña incorrectos.");

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