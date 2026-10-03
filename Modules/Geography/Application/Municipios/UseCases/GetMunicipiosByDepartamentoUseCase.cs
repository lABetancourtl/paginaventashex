using PaginaVentasNet.Api.Modules.Geography.Application.Municipios.Dtos;
using PaginaVentasNet.Api.Modules.Geography.Application.Municipios.Ports;

namespace PaginaVentasNet.Api.Modules.Geography.Application.Municipios.UseCases;

/// <summary>
/// Caso de uso para obtener los municipios de un departamento específico.
/// </summary>
public class GetMunicipiosByDepartamentoUseCase
{
    private readonly IMunicipioRepository _repository;

    public GetMunicipiosByDepartamentoUseCase(IMunicipioRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MunicipioDto>> ExecuteAsync(int departamentoCodigo)
    {
        return await _repository.GetByDepartamentoAsync(departamentoCodigo);
    }
}