using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Lista todas las secciones de la página de inicio, activas o no, para el panel admin.
/// </summary>
public class GetHomeSectionsUseCase
{
    private readonly IHomeSectionRepository _repository;

    public GetHomeSectionsUseCase(IHomeSectionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<HomeSectionResponseDto>> ExecuteAsync()
    {
        return await _repository.GetAllDtosAsync();
    }
}