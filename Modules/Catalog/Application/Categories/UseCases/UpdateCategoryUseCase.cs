using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Categories.UseCases;

public class UpdateCategoryUseCase
{
    private readonly ICategoryRepository _repository;

    public UpdateCategoryUseCase(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _repository.GetEntityByIdAsync(id);

        if (category is null)
            return false;   

        category.Update(dto.Name, dto.Slug);

        await _repository.UpdateAsync(category);
        await _repository.SaveChangesAsync();

        return true;
    }
}