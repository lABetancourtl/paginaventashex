using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Ports;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Addresses.UseCases;

/// <summary>
/// Caso de uso para actualizar una dirección existente del usuario.
/// </summary>
public class UpdateDireccionUseCase
{
    private readonly IDireccionRepository _repository;
    private readonly IGeographyValidator _geographyValidator;

    public UpdateDireccionUseCase(IDireccionRepository repository, IGeographyValidator geographyValidator)
    {
        _repository = repository;
        _geographyValidator = geographyValidator;
    }

    public async Task ExecuteAsync(int usuarioId, int direccionId, UpdateDireccionDto dto)
    {
        
        var direccion = await _repository.GetEntityByIdAsync(direccionId);

        if (direccion is null || direccion.UsuarioId != usuarioId)
            throw new InvalidOperationException(
                $"No existe una dirección con Id '{direccionId}' para este usuario.");

        // 2. ¿Los datos nuevos son válidos?
        var municipioValido = await _geographyValidator
            .MunicipioPerteneceAlDepartamentoAsync(dto.MunicipioCodigo, dto.DepartamentoCodigo);

        if (!municipioValido)
            throw new ArgumentException(
                "El municipio no pertenece al departamento seleccionado.");

        direccion.Update(
            dto.DepartamentoCodigo,
            dto.MunicipioCodigo,
            dto.DireccionTexto,
            dto.NombreQuienRecibe,
            dto.InformacionAdicional,
            dto.Barrio);

        await _repository.SaveChangesAsync();
    }
}