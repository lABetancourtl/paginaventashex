using Microsoft.Extensions.Logging;
using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Quita la imagen para celular de un medio y borra su archivo de Cloudinary.
/// Después, en celular se muestra la imagen principal.
/// </summary>
public class DeleteHomeSectionItemMobileUseCase
{
    private readonly IHomeSectionItemRepository _items;
    private readonly ICloudinaryService _cloudinary;
    private readonly ILogger<DeleteHomeSectionItemMobileUseCase> _logger;

    public DeleteHomeSectionItemMobileUseCase(
        IHomeSectionItemRepository items,
        ICloudinaryService cloudinary,
        ILogger<DeleteHomeSectionItemMobileUseCase> logger)
    {
        _items = items;
        _cloudinary = cloudinary;
        _logger = logger;
    }

    public async Task ExecuteAsync(int sectionId, int itemId)
    {
        var item = await _items.GetEntityByIdAsync(itemId);

        if (item is null || item.SectionId != sectionId)
            throw new AppException("HOME_ITEM_NOT_FOUND",
                $"No se encontró el medio con Id {itemId} en esta sección.", 404);

        var publicId = item.MobilePublicId;

        if (publicId is null)
            throw new AppException("HOME_ITEM_MOBILE_NOT_FOUND",
                "Este medio no tiene imagen para celular.", 404);

        item.ClearMobileImage();
        await _items.SaveChangesAsync();

        try
        {
            await _cloudinary.DeleteAsync(publicId, MediaType.Image);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Se quitó la imagen para celular del medio {ItemId}, pero no se pudo borrar {PublicId} de Cloudinary.",
                itemId, publicId);
        }
    }
}
