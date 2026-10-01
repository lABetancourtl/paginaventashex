using System;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.UseCases;

/// <summary>
/// Caso de uso para eliminar un archivo multimedia de un producto dado su ID y el ID del archivo multimedia.   
/// </summary>
public class DeleteProductMediaUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IProductMediaRepository _mediaRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public DeleteProductMediaUseCase(
        IProductRepository productRepository,
        IProductMediaRepository mediaRepository,
        ICloudinaryService cloudinaryService)
    {
        _productRepository = productRepository;
        _mediaRepository = mediaRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<bool> DeleteAsync(int productId, int mediaId)
    {
        var product = await _productRepository.GetEntityByIdAsync(productId);

        if (product is null)
            throw new InvalidOperationException(
                $"No existe un producto con Id '{productId}'.");

        var media = await _mediaRepository.GetByIdAsync(mediaId);

        if (media is null || media.ProductId != productId)
            throw new InvalidOperationException(
                $"No existe un archivo multimedia con Id '{mediaId}' para el producto con Id '{productId}'.");

        if (media.IsMain && media.MediaType == MediaType.Image)
        {
            var nextImage = await _mediaRepository.GetNextImageAsync(productId, mediaId);
            if (nextImage is not null)
                nextImage.SetAsMain();
        }

        await _cloudinaryService.DeleteAsync(media.PublicId);
        await _mediaRepository.RemoveAsync(media);

        await _mediaRepository.SaveChangesAsync();
        return true;
    }
}