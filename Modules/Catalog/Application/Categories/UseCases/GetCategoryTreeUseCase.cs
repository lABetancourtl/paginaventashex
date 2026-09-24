using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Categories.UseCases;

/// <summary>
/// Caso de uso para obtener todas las categorías en estructura jerárquica.
/// </summary>
public class GetCategoryTreeUseCase
{
    private readonly ICategoryRepository _repository;

    public GetCategoryTreeUseCase(ICategoryRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retorna todas las categorías activas organizadas en árbol jerárquico.
    /// </summary>
    public async Task<List<CategoryTreeDto>> ExecuteAsync()
    {
        return await _repository.GetTreeAsync();
    }
}