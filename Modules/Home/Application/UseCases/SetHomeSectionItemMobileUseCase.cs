using Microsoft.Extensions.Logging;
using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Pone (o reemplaza) la imagen para celular del único medio de un banner.
/// Solo se permite si el banner tiene un medio: con varios, el banner es un carrusel.
/// La URL guardada pide a Cloudinary un recorte 4:5 centrado en lo importante de la foto,
/// así que en celular siempre se ve en ese formato aunque la imagen subida tenga otro.
/// </summary>
public class SetHomeSectionItemMobileUseCase
{
    private const string Folder = "paginaventas/home";
    private const long MaxImageBytes = 10L * 1024 * 1024;
    private const string MobileTransformation = "c_fill,ar_4:5,g_auto";

    private static readonly HashSet<string> ImageTypes = new(StringComparer.OrdinalIgnoreCase)
        { "image/jpeg", "image/png", "image/webp", "image/gif" };

    private readonly IHomeSectionItemRepository _items;
    private readonly ICloudinaryService _cloudinary;
    private readonly ILogger<SetHomeSectionItemMobileUseCase> _logger;

    public SetHomeSectionItemMobileUseCase(
        IHomeSectionItemRepository items,
        ICloudinaryService cloudinary,
        ILogger<SetHomeSectionItemMobileUseCase> logger)
    {
        _items = items;
        _cloudinary = cloudinary;
        _logger = logger;
    }

    public async Task<HomeSectionItemResponseDto> ExecuteAsync(
        int sectionId, int itemId, SetHomeSectionItemMobileDto dto)
    {
        var all = await _items.GetBySectionIdAsync(sectionId);
        var item = all.FirstOrDefault(i => i.Id == itemId);

        if (item is null)
            throw new AppException("HOME_ITEM_NOT_FOUND",
                $"No se encontró el medio con Id {itemId} en esta sección.", 404);

        if (all.Count != 1)
            throw new AppException("HOME_ITEM_MOBILE_NOT_ALLOWED",
                "La imagen para celular solo se puede agregar cuando el banner tiene un único medio.", 409);

        ValidateImage(dto.File);

        string url;
        string publicId;

        try
        {
            (url, publicId) = await _cloudinary.UploadAsync(dto.File, MediaType.Image, Folder);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Falló la subida de una imagen para celular del inicio a Cloudinary.");
            throw new AppException("HOME_ITEM_UPLOAD_FAILED",
                "No se pudo subir el archivo. Intenta de nuevo.", 502);
        }

        var previousPublicId = item.MobilePublicId;

        try
        {
            item.SetMobileImage(ApplyMobileCrop(url), publicId);
            await _items.SaveChangesAsync();
        }
        catch
        {
            await TryDeleteFileAsync(publicId);
            throw;
        }

        // La imagen anterior ya no se usa
        if (previousPublicId is not null)
            await TryDeleteFileAsync(previousPublicId);

        var dtos = await _items.GetDtosBySectionIdAsync(sectionId);
        return dtos.First(d => d.Id == item.Id);
    }

    private static void ValidateImage(Microsoft.AspNetCore.Http.IFormFile file)
    {
        if (file is null || file.Length == 0)
            throw new AppException("HOME_ITEM_INVALID_FILE", "El archivo está vacío.", 400);

        if (!ImageTypes.Contains(file.ContentType))
            throw new AppException("HOME_ITEM_INVALID_FILE",
                "La versión para celular debe ser una imagen: JPEG, PNG, WebP o GIF.", 400);

        if (file.Length > MaxImageBytes)
            throw new AppException("HOME_ITEM_INVALID_FILE",
                $"La imagen no puede superar {MaxImageBytes / (1024 * 1024)} MB.", 400);
    }

    /// <summary>Inserta el recorte 4:5 en la URL de entrega de Cloudinary.</summary>
    private static string ApplyMobileCrop(string url)
    {
        const string marker = "/upload/";
        var index = url.IndexOf(marker, StringComparison.Ordinal);

        return index < 0
            ? url
            : url.Insert(index + marker.Length, MobileTransformation + "/");
    }

    private async Task TryDeleteFileAsync(string publicId)
    {
        try
        {
            await _cloudinary.DeleteAsync(publicId, MediaType.Image);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "No se pudo borrar de Cloudinary la imagen para celular {PublicId}.", publicId);
        }
    }
}
