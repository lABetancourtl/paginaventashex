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
}