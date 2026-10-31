using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Colors.UseCases;

/// <summary>
/// Lista los colores ordenados por nombre. Por defecto solo los activos.
/// </summary>
public class GetColorsUseCase
{
    private readonly IColorRepository _repository;

    public GetColorsUseCase(IColorRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ColorResponseDto>> ExecuteAsync(bool includeInactive)
    {
        return await _repository.GetAllAsync(includeInactive);
    }
}