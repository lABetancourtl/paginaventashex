using PaginaVentasNet.Api.Modules.Geography.Application.Departamentos.Dtos;

namespace PaginaVentasNet.Api.Modules.Geography.Application.Departamentos.Ports;

public interface IDepartamentoRepository
{
    Task<List<DepartamentoDto>> GetAllAsync();
}