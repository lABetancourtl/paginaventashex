using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Categories.UseCases;

/// <summary> 
/// Caso de uso para obtener todas las categorías.
/// </summary>
public class GetCategoriesUseCase
{
    private readonly ICategoryRepository _repository;

    public GetCategoriesUseCase(ICategoryRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Obtiene todas las categorías.
    /// </summary>
    /// <returns>Lista de categorías.</returns>
    public async Task<List<CategoryResponseDto>> ExecuteAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <summary>
    /// Obtiene una categoría por su Id.
    /// </summary>
    /// <param name="id">Id de la categoría a buscar.</param>
    /// <returns>Datos de la categoría o null si no existe.</returns>
    public async Task<CategoryResponseDto?> ExecuteAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }
}