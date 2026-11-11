using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Cambia el enlace de un medio. Reemplaza el enlace: si no se envía categoryId,
/// el medio deja de ser un enlace.
/// </summary>
public class UpdateHomeSectionItemUseCase
{
    private readonly IHomeSectionItemRepository _items;
    private readonly ICategoryRepository _categories;

    public UpdateHomeSectionItemUseCase(
        IHomeSectionItemRepository items,
        ICategoryRepository categories)
    {
        _items = items;
        _categories = categories;
    }

    public async Task ExecuteAsync(int sectionId, int itemId, UpdateHomeSectionItemDto dto)
    {
        var item = await _items.GetEntityByIdAsync(itemId);

        if (item is null || item.SectionId != sectionId)
            throw new AppException("HOME_ITEM_NOT_FOUND",
                $"No se encontró el medio con Id {itemId} en esta sección.", 404);

        if (dto.CategoryId.HasValue && !await _categories.ExistsByIdAsync(dto.CategoryId.Value))
            throw new AppException("HOME_ITEM_VALIDATION",
                $"No existe la categoría con Id {dto.CategoryId}.", 400);

        item.SetCategory(dto.CategoryId);
        await _items.SaveChangesAsync();
    }
}