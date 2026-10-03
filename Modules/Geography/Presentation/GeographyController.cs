using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Geography.Application.Departamentos.Dtos;
using PaginaVentasNet.Api.Modules.Geography.Application.Departamentos.UseCases;
using PaginaVentasNet.Api.Modules.Geography.Application.Municipios.Dtos;
using PaginaVentasNet.Api.Modules.Geography.Application.Municipios.UseCases;

namespace PaginaVentasNet.Api.Modules.Geography.Presentation;

/// <summary>
/// Controller para consultar departamentos y municipios de Colombia según DIVIPOLA.
/// </summary>
[Route("api/geography")]
public class GeographyController : ApiController
{
    private readonly GetDepartamentosUseCase _getDepartamentosUseCase;
    private readonly GetMunicipiosByDepartamentoUseCase _getMunicipiosUseCase;

    public GeographyController(
        GetDepartamentosUseCase getDepartamentosUseCase,
        GetMunicipiosByDepartamentoUseCase getMunicipiosUseCase)
    {
        _getDepartamentosUseCase = getDepartamentosUseCase;
        _getMunicipiosUseCase = getMunicipiosUseCase;
    }

    /// <summary>
    /// Obtiene todos los departamentos de Colombia ordenados alfabéticamente.
    /// </summary>
    [HttpGet("departamentos")]
    public async Task<ActionResult<ApiResponse<List<DepartamentoDto>>>> GetDepartamentos()
    {
        var departamentos = await _getDepartamentosUseCase.ExecuteAsync();
        return Success(departamentos);
    }

    /// <summary>
    /// Obtiene los municipios de un departamento específico ordenados alfabéticamente.
    /// </summary>
    /// <param name="codigo">Código DIVIPOLA del departamento.</param>
    [HttpGet("departamentos/{codigo}/municipios")]
    public async Task<ActionResult<ApiResponse<List<MunicipioDto>>>> GetMunicipios(int codigo)
    {
        var municipios = await _getMunicipiosUseCase.ExecuteAsync(codigo);
        return Success(municipios);
    }
}