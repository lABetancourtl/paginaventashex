using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Ports;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Addresses.UseCases;

/// <summary>
/// Caso de uso para eliminar una dirección del usuario.
/// Si se elimina la principal, asigna la siguiente disponible como principal.
/// </summary>
public class DeleteDireccionUseCase
{
    private readonly IDireccionRepository _repository;

    public DeleteDireccionUseCase(IDireccionRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Elimina una dirección del usuario y maneja la lógica de asignación de la dirección principal si es necesario.
    /// Si la dirección eliminada era la principal, se asigna la siguiente dirección disponible como principal.
    /// Si no hay más direcciones, no se asigna ninguna como principal.
    /// </summary>
    /// <param name="usuarioId"></param>
    /// <param name="direccionId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task ExecuteAsync(int usuarioId, int direccionId)
    {
        var direccion = await _repository.GetEntityByIdAsync(direccionId);

        if (direccion is null || direccion.UsuarioId != usuarioId)
            throw new InvalidOperationException(
                $"No existe una dirección con Id '{direccionId}' para este usuario.");

        var eraPrincipal = direccion.EsPrincipal;

        await _repository.RemoveAsync(direccion);
        await _repository.SaveChangesAsync();

        if (eraPrincipal)
        {
            var siguiente = await _repository.GetPrincipalByUsuarioIdAsync(usuarioId);
            if (siguiente is not null)
            {
                siguiente.SetAsPrincipal();
                await _repository.SaveChangesAsync();
            }
        }
    }
}