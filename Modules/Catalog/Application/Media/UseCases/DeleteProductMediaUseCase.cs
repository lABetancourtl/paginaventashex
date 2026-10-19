using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.UseCases;

/// <summary>
/// Use case for deleting a media file associated with a product. It also reorders the remaining media files of the same type after deletion.
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

    /// <summary>
    /// Deletes a media file associated with a product and reorders the remaining media files of the same type. 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="mediaId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
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

        var deletedOrder = media.DisplayOrder;
        var deletedType = media.MediaType;

        await _cloudinaryService.DeleteAsync(media.PublicId);
        await _mediaRepository.RemoveAsync(media);
        await _mediaRepository.SaveChangesAsync();


        var remaining = await _mediaRepository.GetByProductIdAsync(productId);
        var toReorder = remaining
            .Where(m => m.MediaType == deletedType && m.DisplayOrder > deletedOrder)
            .OrderBy(m => m.DisplayOrder)
            .ToList();

        foreach (var item in toReorder)
            item.SetDisplayOrder(item.DisplayOrder - 1);

        if (toReorder.Any())
            await _mediaRepository.SaveChangesAsync();

        return true;
    }
}