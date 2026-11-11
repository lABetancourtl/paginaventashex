using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;
using PaginaVentasNet.Api.Modules.Home.Domain;
using PaginaVentasNet.Api.Modules.Home.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Agrega un medio (imagen o video) al final de un banner. El tipo se deduce del archivo.
/// Primero se sube a Cloudinary y después se guarda en la base; si guardar falla,
/// se borra el archivo recién subido.
/// </summary>
public class AddHomeSectionItemUseCase
{
    private const string Folder = "paginaventas/home";
    private const long MaxImageBytes = 10L * 1024 * 1024;
    private const long MaxVideoBytes = 50L * 1024 * 1024;

    private static readonly HashSet<string> ImageTypes = new(StringComparer.OrdinalIgnoreCase)
        { "image/jpeg", "image/png", "image/webp", "image/gif" };

    private static readonly HashSet<string> VideoTypes = new(StringComparer.OrdinalIgnoreCase)
        { "video/mp4", "video/webm", "video/quicktime" };

    private readonly IHomeSectionRepository _sections;
    private readonly IHomeSectionItemRepository _items;
    private readonly ICategoryRepository _categories;
    private readonly ICloudinaryService _cloudinary;
    private readonly ILogger<AddHomeSectionItemUseCase> _logger;

    public AddHomeSectionItemUseCase(
        IHomeSectionRepository sections,
        IHomeSectionItemRepository items,
        ICategoryRepository categories,
        ICloudinaryService cloudinary,
        ILogger<AddHomeSectionItemUseCase> logger)
    {
        _sections = sections;
        _items = items;
        _categories = categories;
        _cloudinary = cloudinary;
        _logger = logger;
    }

    public async Task<HomeSectionItemResponseDto> ExecuteAsync(int sectionId, AddHomeSectionItemDto dto)
    {
        var section = await _sections.GetEntityByIdAsync(sectionId);

        if (section is null)
            throw new AppException("HOME_SECTION_NOT_FOUND",
                $"No se encontró la sección con Id {sectionId}.", 404);

        if (section.Type != HomeSectionType.Banner)
            throw new AppException("HOME_SECTION_INVALID_TYPE",
                "Solo los banners tienen medios.", 400);

        var existing = await _items.GetBySectionIdAsync(sectionId);

        if (existing.Count >= HomeSection.MaxBannerItems)
            throw new AppException("HOME_ITEM_LIMIT",
                $"Un banner admite hasta {HomeSection.MaxBannerItems} medios.", 409);

        if (dto.CategoryId.HasValue && !await _categories.ExistsByIdAsync(dto.CategoryId.Value))
            throw new AppException("HOME_ITEM_VALIDATION",
                $"No existe la categoría con Id {dto.CategoryId}.", 400);

        var mediaType = ResolveMediaType(dto.File);

        string url;
        string publicId;

        try
        {
            (url, publicId) = await _cloudinary.UploadAsync(dto.File, mediaType, Folder);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Falló la subida de un medio del inicio a Cloudinary.");
            throw new AppException("HOME_ITEM_UPLOAD_FAILED",
                "No se pudo subir el archivo. Intenta de nuevo.", 502);
        }

        HomeSectionItem item;

        try
        {
            item = HomeSectionItem.Create(
                sectionId, url, publicId, mediaType, dto.CategoryId, existing.Count + 1);

            await _items.AddAsync(item);
            await _items.SaveChangesAsync();
        }
        catch
        {
            await TryDeleteFileAsync(publicId, mediaType);
            throw;
        }

        var dtos = await _items.GetDtosBySectionIdAsync(sectionId);
        return dtos.First(d => d.Id == item.Id);
    }

    private static MediaType ResolveMediaType(IFormFile file)
    {
        if (file is null || file.Length == 0)
            throw new AppException("HOME_ITEM_INVALID_FILE", "El archivo está vacío.", 400);

        if (ImageTypes.Contains(file.ContentType))
        {
            if (file.Length > MaxImageBytes)
                throw new AppException("HOME_ITEM_INVALID_FILE",
                    $"La imagen no puede superar {MaxImageBytes / (1024 * 1024)} MB.", 400);

            return MediaType.Image;
        }

        if (VideoTypes.Contains(file.ContentType))
        {
            if (file.Length > MaxVideoBytes)
                throw new AppException("HOME_ITEM_INVALID_FILE",
                    $"El video no puede superar {MaxVideoBytes / (1024 * 1024)} MB.", 400);

            return MediaType.Video;
        }

        throw new AppException("HOME_ITEM_INVALID_FILE",
            "Formato no permitido. Imágenes: JPEG, PNG, WebP o GIF. Videos: MP4, WebM o MOV.", 400);
    }

    private async Task TryDeleteFileAsync(string publicId, MediaType mediaType)
    {
        try
        {
            await _cloudinary.DeleteAsync(publicId, mediaType);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "No se pudo borrar de Cloudinary el archivo {PublicId} tras un error al guardar.", publicId);
        }
    }
}