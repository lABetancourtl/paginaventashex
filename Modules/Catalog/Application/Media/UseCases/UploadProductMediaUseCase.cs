using Microsoft.AspNetCore.Http;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.UseCases;

/// <summary>
/// Caso de uso para subir un archivo multimedia a un producto.
/// Reglas: máximo 9 imágenes y 1 video por producto.
/// Primera imagen → CatalogMain, segunda → CatalogHover, resto → None.
/// </summary>
public class UploadProductMediaUseCase
{
    private const int MaxImages = 9;
    private readonly IProductRepository _productRepository;
    private readonly IProductMediaRepository _mediaRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public UploadProductMediaUseCase(
        IProductRepository productRepository,
        IProductMediaRepository mediaRepository,
        ICloudinaryService cloudinaryService)
    {
        _productRepository = productRepository;
        _mediaRepository = mediaRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ProductMediaResponseDto> ExecuteAsync(
        int productId,
        IFormFile file,
        MediaType mediaType)
    {
        var product = await _productRepository.GetEntityByIdAsync(productId);

        if (product is null)
            throw new InvalidOperationException(
                $"No existe un producto con Id '{productId}'.");

        if (mediaType == MediaType.Image)
        {
            var totalImages = await _mediaRepository.CountImagesByProductIdAsync(productId);
            if (totalImages >= MaxImages)
                throw new InvalidOperationException(
                    $"El producto ya tiene el máximo de {MaxImages} imágenes permitidas.");
        }

        if (mediaType == MediaType.Video)
        {
            var hasVideo = await _mediaRepository.HasVideoAsync(productId);
            if (hasVideo)
                throw new InvalidOperationException(
                    "El producto ya tiene un video. Solo se permite 1 video por producto.");
        }

        var (url, publicId) = await _cloudinaryService.UploadAsync(file, mediaType);

        var existingMedia = await _mediaRepository.GetByProductIdAsync(productId);
        var displayOrder = existingMedia.Count + 1;

        var displayRole = DisplayRole.None;
        if (mediaType == MediaType.Image)
        {
            var imageCount = existingMedia.Count(m => m.MediaType == MediaType.Image);
            displayRole = imageCount switch
            {
                0 => DisplayRole.CatalogMain,
                1 => DisplayRole.CatalogHover,
                _ => DisplayRole.None
            };
        }

        var media = ProductMedia.Create(
            productId, url, publicId, mediaType, displayOrder, displayRole);

        await _mediaRepository.AddAsync(media);
        await _mediaRepository.SaveChangesAsync();

        return new ProductMediaResponseDto
        {
            Id = media.Id,
            Url = media.Url,
            MediaType = media.MediaType,
            DisplayRole = media.DisplayRole,
            DisplayOrder = media.DisplayOrder
        };
    }
}