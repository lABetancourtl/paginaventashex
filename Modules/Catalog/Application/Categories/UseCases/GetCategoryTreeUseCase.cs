using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Categories.UseCases;

/// <summary>
/// Caso de uso para obtener las categorías en estructura jerárquica.
/// </summary>
public class GetCategoryTreeUseCase
{
    private readonly ICategoryRepository _repository;

    public GetCategoryTreeUseCase(ICategoryRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Con includeInactive = false (tienda pública) retorna solo las categorías activas.
    /// Con includeInactive = true (panel admin) retorna todas, activas o no.
    /// </summary>
    public async Task<List<CategoryTreeDto>> ExecuteAsync(bool includeInactive = false)
    {
        return await _repository.GetTreeAsync(includeInactive);
    }
}