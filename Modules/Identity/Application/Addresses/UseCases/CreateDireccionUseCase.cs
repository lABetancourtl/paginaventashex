using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Ports;
using PaginaVentasNet.Api.Modules.Identity.Domain;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Addresses.UseCases;

/// <summary>
/// Caso de uso para agregar una nueva dirección al usuario.
/// Si es la primera dirección, se marca automáticamente como principal.
/// </summary>
public class CreateDireccionUseCase
{
    private readonly IDireccionRepository _repository;

    public CreateDireccionUseCase(IDireccionRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> ExecuteAsync(int usuarioId, CreateDireccionDto dto)
    {
        var direcciones = await _repository.GetByUsuarioIdAsync(usuarioId);
        var esPrincipal = direcciones.Count == 0;

        var direccion = Direccion.Create(
            usuarioId,
            dto.DepartamentoCodigo,
            dto.MunicipioCodigo,
            dto.DireccionTexto,
            dto.NombreQuienRecibe,
            dto.InformacionAdicional,
            dto.Barrio,
            esPrincipal);

        await _repository.AddAsync(direccion);
        await _repository.SaveChangesAsync();

        return direccion.Id;
    }
}