using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Ports;
using PaginaVentasNet.Api.Modules.Identity.Application.Users.Dtos;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Users.UseCases;

/// <summary>
/// Caso de uso para cambiar el rol de un usuario.
/// Solo puede ser ejecutado por un administrador.
/// </summary>
public class ChangeRolUseCase
{
    private readonly IUsuarioRepository _repository;

    public ChangeRolUseCase(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Cambia el rol de un usuario.
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public async Task ExecuteAsync(int usuarioId, ChangeRolDto dto)
    {
        var usuario = await _repository.GetByIdAsync(usuarioId);

        if (usuario is null)
            throw new InvalidOperationException(
                $"No existe un usuario con Id '{usuarioId}'.");

        if (usuario.Rol == dto.Rol)
            throw new ArgumentException(
                $"El usuario ya tiene el rol '{dto.Rol}'.");

        usuario.ChangeRol(dto.Rol);

        await _repository.UpdateAsync(usuario);
        await _repository.SaveChangesAsync();
    }
}