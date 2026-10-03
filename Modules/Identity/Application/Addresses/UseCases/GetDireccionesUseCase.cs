using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Ports;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Addresses.UseCases;

/// <summary>
/// Caso de uso para obtener todas las direcciones del usuario autenticado.
/// </summary>
public class GetDireccionesUseCase
{
    private readonly IDireccionRepository _repository;

    public GetDireccionesUseCase(IDireccionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DireccionResponseDto>> ExecuteAsync(int usuarioId)
    {
        return await _repository.GetByUsuarioIdAsync(usuarioId);
    }
}