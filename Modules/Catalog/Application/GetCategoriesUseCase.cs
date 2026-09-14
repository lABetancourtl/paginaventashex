namespace PaginaVentasNet.Api.Modules.Catalog.Application;

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