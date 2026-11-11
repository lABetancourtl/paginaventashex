using Microsoft.Extensions.Logging;
using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Quita un medio de un banner y borra su archivo de Cloudinary. Los siguientes suben un puesto.
/// Primero se guarda en la base: si Cloudinary falla después, el medio ya salió de la página
/// y queda un aviso en el log.
/// </summary>
public class DeleteHomeSectionItemUseCase
{
    private readonly IHomeSectionItemRepository _items;
    private readonly ICloudinaryService _cloudinary;
    private readonly ILogger<DeleteHomeSectionItemUseCase> _logger;

    public DeleteHomeSectionItemUseCase(
        IHomeSectionItemRepository items,
        ICloudinaryService cloudinary,
        ILogger<DeleteHomeSectionItemUseCase> logger)
    {
        _items = items;
        _cloudinary = cloudinary;
        _logger = logger;
    }

    public async Task ExecuteAsync(int sectionId, int itemId)
    {
        var all = await _items.GetBySectionIdAsync(sectionId);
        var item = all.FirstOrDefault(i => i.Id == itemId);

        if (item is null)
            throw new AppException("HOME_ITEM_NOT_FOUND",
                $"No se encontró el medio con Id {itemId} en esta sección.", 404);

        var publicId = item.PublicId;
        var mediaType = item.MediaType;
        var deletedOrder = item.DisplayOrder;

        await _items.RemoveAsync(item);

        foreach (var following in all.Where(i => i.DisplayOrder > deletedOrder))
            following.SetDisplayOrder(following.DisplayOrder - 1);

        await _items.SaveChangesAsync();

        try
        {
            await _cloudinary.DeleteAsync(publicId, mediaType);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "El medio {ItemId} se quitó, pero no se pudo borrar {PublicId} de Cloudinary.",
                itemId, publicId);
        }
    }
}