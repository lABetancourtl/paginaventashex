using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Ports;
using PaginaVentasNet.Api.Modules.Identity.Application.Profile.Dtos;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Profile.UseCases;

/// <summary>
/// Caso de uso para asignar o cambiar la contraseña del usuario autenticado.
/// </summary>
public class UpdatePasswordUseCase
{
    private readonly IUsuarioRepository _repository;

    public UpdatePasswordUseCase(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(int usuarioId, UpdatePasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            throw new ArgumentException("Las contraseñas no coinciden.");

        var usuario = await _repository.GetByIdAsync(usuarioId);

        if (usuario is null)
            throw new InvalidOperationException("Usuario no encontrado.");

        var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        usuario.UpdatePassword(newPasswordHash);

        await _repository.UpdateAsync(usuario);
        await _repository.SaveChangesAsync();
    }
}