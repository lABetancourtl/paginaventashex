using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Categories.UseCases;

public class GetCategoriesUseCase
{
    private readonly ICategoryRepository _repository;

    public GetCategoriesUseCase(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CategoryResponseDto>> ExecuteAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <summary>
    /// Obtiene una categoría por su Id.
    /// </summary>
    /// <param name="id">Id de la categoría a buscar.</param>
    /// <returns>Datos de la categoría o null si no existe.</returns>
    internal async Task<CategoryResponseDto?> ExecuteAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }
}