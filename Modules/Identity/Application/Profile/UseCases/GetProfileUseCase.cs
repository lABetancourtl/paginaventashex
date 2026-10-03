using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Ports;
using PaginaVentasNet.Api.Modules.Identity.Application.Profile.Dtos;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Profile.UseCases;

/// <summary>
/// Caso de uso para obtener el perfil del usuario autenticado.
/// </summary>
public class GetProfileUseCase
{
    private readonly IUsuarioRepository _repository;

    public GetProfileUseCase(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProfileResponseDto> ExecuteAsync(int usuarioId)
    {
        var usuario = await _repository.GetByIdAsync(usuarioId);

        if (usuario is null)
            throw new InvalidOperationException("Usuario no encontrado.");

        return new ProfileResponseDto
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Rol = usuario.Rol,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Documento = usuario.Documento,
            Genero = usuario.Genero,
            FechaNacimiento = usuario.FechaNacimiento,
            Telefono = usuario.Telefono,
            TienePassword = !string.IsNullOrEmpty(usuario.PasswordHash),
            CreadoEn = usuario.CreadoEn
        };
    }
}