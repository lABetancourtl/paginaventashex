using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Edita el título y el botón "Ver todo" de una sección de productos.
/// Reemplaza ambos campos: si no se envían, quedan en null.
/// </summary>
public class UpdateHomeSectionUseCase
{
    private readonly IHomeSectionRepository _repository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateHomeSectionUseCase(
        IHomeSectionRepository repository,
        ICategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    public async Task ExecuteAsync(int id, UpdateHomeSectionDto dto)
    {
        var section = await _repository.GetEntityByIdAsync(id);

        if (section is null)
            throw new AppException("HOME_SECTION_NOT_FOUND",
                $"No se encontró la sección con Id {id}.", 404);

        if (dto.CategoryId.HasValue && !await _categoryRepository.ExistsByIdAsync(dto.CategoryId.Value))
            throw new AppException("HOME_SECTION_VALIDATION",
                $"No existe la categoría con Id {dto.CategoryId}.", 400);

        try
        {
            section.Update(dto.Title, dto.CategoryId);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            throw new AppException("HOME_SECTION_VALIDATION", ex.Message, 400);
        }

        await _repository.SaveChangesAsync();
    }
}