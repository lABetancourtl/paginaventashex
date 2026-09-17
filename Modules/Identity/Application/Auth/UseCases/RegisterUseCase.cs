using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Ports;
using PaginaVentasNet.Api.Modules.Identity.Domain;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Auth.UseCases;

public class RegisterUseCase
{
    private readonly IUsuarioRepository _repository;
    private readonly IJwtService _jwtService;

    public RegisterUseCase(IUsuarioRepository repository, IJwtService jwtService)
    {
        _repository = repository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> ExecuteAsync(RegisterDto dto)
    {
        var emailExiste = await _repository.ExistsByEmailAsync(dto.Email);

        if (emailExiste)
            throw new InvalidOperationException("El email ya está registrado.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var usuario = Usuario.Create(dto.Email, passwordHash);

        await _repository.AddAsync(usuario);
        await _repository.SaveChangesAsync();

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