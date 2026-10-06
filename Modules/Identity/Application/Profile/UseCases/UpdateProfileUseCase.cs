using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Ports;
using PaginaVentasNet.Api.Modules.Identity.Application.Profile.Dtos;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Profile.UseCases;

/// <summary>
/// Caso de uso para actualizar los datos del perfil del usuario autenticado.
/// </summary>
public class UpdateProfileUseCase
{
    private readonly IUsuarioRepository _repository;

    public UpdateProfileUseCase(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(int usuarioId, UpdateProfileDto dto)
    {
        var usuario = await _repository.GetByIdAsync(usuarioId);

        if (usuario is null)
            throw new InvalidOperationException("Usuario no encontrado.");

        usuario.UpdateProfile(
            dto.Nombre,
            dto.Apellido,
            dto.Documento,
            dto.Genero,
            dto.FechaNacimiento,
            dto.Telefono);

        await _repository.UpdateAsync(usuario);
        await _repository.SaveChangesAsync();
    }

    public async Task DeactivateProfileAsync(int usuarioId)
    {
        var usuario = await _repository.GetByIdAsync(usuarioId);

        if (usuario is null)
            throw new InvalidOperationException("Usuario no encontrado.");

        usuario.Deactivate();
        
        await _repository.UpdateAsync(usuario);
        await _repository.SaveChangesAsync();
    }
}