using PaginaVentasNet.Api.Modules.Geography.Application.Municipios.Dtos;

namespace PaginaVentasNet.Api.Modules.Geography.Application.Municipios.Ports;

public interface IMunicipioRepository
{
    Task<List<MunicipioDto>> GetByDepartamentoAsync(int departamentoCodigo);
}