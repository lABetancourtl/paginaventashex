using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;
using PaginaVentasNet.Api.Modules.Home.Domain;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Crea una sección al final de la página de inicio. Nace inactiva: el admin la llena y luego la activa.
/// </summary>
public class CreateHomeSectionUseCase
{
    private readonly IHomeSectionRepository _repository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateHomeSectionUseCase(
        IHomeSectionRepository repository,
        ICategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    public async Task<int> ExecuteAsync(CreateHomeSectionDto dto)
    {
        var displayOrder = await _repository.GetNextDisplayOrderAsync();

        HomeSection section;

        try
        {
            section = HomeSection.Create(dto.Type!.Value, dto.Title, dto.CategoryId, displayOrder);
        }
        catch (ArgumentException ex)
        {
            throw new AppException("HOME_SECTION_VALIDATION", ex.Message, 400);
        }

        if (dto.CategoryId.HasValue && !await _categoryRepository.ExistsByIdAsync(dto.CategoryId.Value))
            throw new AppException("HOME_SECTION_VALIDATION",
                $"No existe la categoría con Id {dto.CategoryId}.", 400);

        await _repository.AddAsync(section);
        await _repository.SaveChangesAsync();

        return section.Id;
    }
}