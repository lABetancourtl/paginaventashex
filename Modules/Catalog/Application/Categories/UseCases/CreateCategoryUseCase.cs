using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Categories.UseCases;

public class CreateCategoryUseCase
{
    private readonly ICategoryRepository _repository;

    public CreateCategoryUseCase(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> ExecuteAsync(CreateCategoryDto dto)
    {
        var slugExiste = await _repository.ExistsBySlugAsync(dto.Slug);

        if (slugExiste)
            throw new InvalidOperationException($"Ya existe una categoría con el slug '{dto.Slug}'.");

        var category = Category.Create(dto.Name, dto.Slug, dto.ParentCategoryId);

        await _repository.AddAsync(category);
        await _repository.SaveChangesAsync();

        return category.Id;
    }
}