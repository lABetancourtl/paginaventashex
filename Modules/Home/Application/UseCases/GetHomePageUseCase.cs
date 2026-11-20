using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Arma la página de inicio pública: las secciones activas, en orden, con solo lo que se ve en la tienda.
/// </summary>
public class GetHomePageUseCase
{
    private readonly IHomeSectionRepository _repository;

    public GetHomePageUseCase(IHomeSectionRepository repository)
    {
        _repository = repository;
    }

    public async Task<HomePageResponseDto> ExecuteAsync()
    {
        var sections = await _repository.GetPublicSectionsAsync();

        return new HomePageResponseDto { Sections = sections };
    }
}