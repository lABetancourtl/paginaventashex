using PaginaVentasNet.Api.Modules.Geography.Application.Departamentos.Dtos;
using PaginaVentasNet.Api.Modules.Geography.Application.Departamentos.Ports;

namespace PaginaVentasNet.Api.Modules.Geography.Application.Departamentos.UseCases;

/// <summary>
/// Caso de uso para obtener todos los departamentos de Colombia.
/// </summary>
public class GetDepartamentosUseCase
{
    private readonly IDepartamentoRepository _repository;

    public GetDepartamentosUseCase(IDepartamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DepartamentoDto>> ExecuteAsync()
    {
        return await _repository.GetAllAsync();
    }
}