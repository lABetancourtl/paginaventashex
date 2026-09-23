using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Categories.UseCases;

/// <summary>
/// Caso de uso para actualizar una categoría existente.
/// </summary>
public class UpdateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Ejecuta el caso de uso para actualizar una categoría existente.
    /// </summary>
    /// <param name="id">Id de la categoría a actualizar.</param>
    /// <param name="dto">Datos de la categoría a actualizar: name y slug.</param>
    /// <returns>True si la categoría fue actualizada correctamente, false si no se encontró la categoría con el Id proporcionado.</returns>
    public async Task<bool> ExecuteAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetEntityByIdAsync(id);

        if (category is null)
            return false;   

        category.Update(dto.Name, dto.Slug);

        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Activa una categoría existente.
    /// </summary>
    /// <param name="id">Id de la categoría a activar.</param>
    /// <returns>True si se activó correctamente, false si no se encontró la categoría  con el Id proporcionado.</returns>
    public async Task<bool> ActivateAsync(int id)
    {
        var category = await _categoryRepository.GetEntityByIdAsync(id);

        if (category is null)
            return false;

        category.Activate();
        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(int id)
    {        
        var category = await _categoryRepository.GetEntityByIdAsync(id);

        if (category is null)
            return false;

        category.Deactivate();
        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return true;
    }
}