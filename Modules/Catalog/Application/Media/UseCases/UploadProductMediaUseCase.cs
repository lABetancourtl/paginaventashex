using Microsoft.AspNetCore.Http;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.UseCases;

/// <summary>
/// Use case for uploading media (images or videos) for a product. This use case ensures that the product exists, validates the media type constraints (maximum number of images and single video), uploads the media to Cloudinary, and creates a media record in the repository with the appropriate display order.   
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

    /// <summary>
    /// Uploads a media file (image or video) for a specific product, ensuring that the product exists and that the media type constraints are respected (maximum number of images and single video). The media is uploaded to Cloudinary, and the media record is created in the repository with the appropriate display order.    
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="file"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
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

        int displayOrder;
        if (mediaType == MediaType.Video)
        {
            displayOrder = existingMedia.Count + 1;
        }
        else
        {
            var imageCount = existingMedia.Count(m => m.MediaType == MediaType.Image);
            displayOrder = imageCount + 1;

            var video = existingMedia.FirstOrDefault(m => m.MediaType == MediaType.Video);
            if (video is not null)
                video.SetDisplayOrder(imageCount + 2);
        }

        var media = ProductMedia.Create(productId, url, publicId, mediaType, displayOrder);

        await _mediaRepository.AddAsync(media);
        await _mediaRepository.SaveChangesAsync();

        return new ProductMediaResponseDto
        {
            Id = media.Id,
            Url = media.Url,
            MediaType = media.MediaType,
            DisplayOrder = media.DisplayOrder
        };
    }
}