using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Ports;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Addresses.UseCases;

/// <summary>
/// Caso de uso para cambiar la dirección principal del usuario.
/// </summary>
public class SetDireccionPrincipalUseCase
{
    private readonly IDireccionRepository _repository;

    public SetDireccionPrincipalUseCase(IDireccionRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Cambia la dirección principal del usuario al especificado por el Id de dirección.
    /// </summary>
    /// <param name="usuarioId">Id del usuario.</param>
    /// <param name="direccionId">Id de la dirección que se desea establecer como principal.</param>
    /// <returns></returns>
    public async Task ExecuteAsync(int usuarioId, int direccionId)
    {
        var nuevaPrincipal = await _repository.GetEntityByIdAsync(direccionId);

        if (nuevaPrincipal is null || nuevaPrincipal.UsuarioId != usuarioId)
            throw new InvalidOperationException(
                $"No existe una dirección con Id '{direccionId}' para este usuario.");

        if (nuevaPrincipal.EsPrincipal)
            throw new ArgumentException("Esta dirección ya es la principal.");

        var actualPrincipal = await _repository.GetPrincipalByUsuarioIdAsync(usuarioId);

        if (actualPrincipal is not null)
            actualPrincipal.RemoveAsPrincipal();

        nuevaPrincipal.SetAsPrincipal();

        await _repository.SaveChangesAsync();
    }
}