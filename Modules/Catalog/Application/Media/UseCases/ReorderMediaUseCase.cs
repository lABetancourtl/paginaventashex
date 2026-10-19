using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.UseCases;

/// <summary>
/// Caso de uso para intercambiar el orden de una imagen con la que ocupa el orden destino.
/// </summary>
public class ReorderMediaUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IProductMediaRepository _mediaRepository;

    public ReorderMediaUseCase(
        IProductRepository productRepository,
        IProductMediaRepository mediaRepository)
    {
        _productRepository = productRepository;
        _mediaRepository = mediaRepository;
    }

    public async Task ExecuteAsync(int productId, ReorderMediaDto dto)
    {
        var product = await _productRepository.GetEntityByIdAsync(productId);

        if (product is null)
            throw new InvalidOperationException(
                $"No existe un producto con Id '{productId}'.");

        var media = await _mediaRepository.GetByIdAsync(dto.MediaId);

        if (media is null || media.ProductId != productId)
            throw new InvalidOperationException(
                $"No existe un archivo multimedia con Id '{dto.MediaId}' para este producto.");

        if (media.MediaType == MediaType.Video)
            throw new ArgumentException(
                "No se puede reordenar el video. El video siempre queda al final.");

        if (media.DisplayOrder == dto.NewDisplayOrder)
            throw new ArgumentException(
                "La imagen ya tiene ese orden.");

        var allMedia = await _mediaRepository.GetByProductIdAsync(productId);
        var displaced = allMedia.FirstOrDefault(m =>
            m.Id != dto.MediaId &&
            m.DisplayOrder == dto.NewDisplayOrder &&
            m.MediaType == MediaType.Image);

        var originalOrder = media.DisplayOrder;

        media.SetDisplayOrder(9999);
        await _mediaRepository.SaveChangesAsync();

        if (displaced is not null)
        {
            displaced.SetDisplayOrder(originalOrder);
            await _mediaRepository.SaveChangesAsync();
        }

        media.SetDisplayOrder(dto.NewDisplayOrder);
        await _mediaRepository.SaveChangesAsync();
    }
}