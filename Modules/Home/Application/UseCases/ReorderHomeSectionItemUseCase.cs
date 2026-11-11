using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Mueve un medio a un nuevo orden dentro de su banner, por intercambio con el que lo ocupa.
/// </summary>
public class ReorderHomeSectionItemUseCase
{
    private readonly IHomeSectionItemRepository _items;

    public ReorderHomeSectionItemUseCase(IHomeSectionItemRepository items)
    {
        _items = items;
    }

    public async Task ExecuteAsync(int sectionId, int itemId, ReorderHomeSectionItemDto dto)
    {
        var all = await _items.GetBySectionIdAsync(sectionId);
        var item = all.FirstOrDefault(i => i.Id == itemId);

        if (item is null)
            throw new AppException("HOME_ITEM_NOT_FOUND",
                $"No se encontró el medio con Id {itemId} en esta sección.", 404);

        if (item.DisplayOrder == dto.NewDisplayOrder)
            throw new AppException("HOME_ITEM_REORDER_INVALID",
                "El medio ya tiene ese orden.", 400);

        var other = all.FirstOrDefault(i => i.DisplayOrder == dto.NewDisplayOrder);

        if (other is null)
            throw new AppException("HOME_ITEM_REORDER_INVALID",
                $"No existe ningún medio con el orden {dto.NewDisplayOrder} en este banner.", 400);

        var originalOrder = item.DisplayOrder;

        other.SetDisplayOrder(originalOrder);
        item.SetDisplayOrder(dto.NewDisplayOrder);

        await _items.SaveChangesAsync();
    }
}