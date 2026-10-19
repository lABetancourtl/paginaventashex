using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.UseCases;

/// <summary>
/// Caso de uso para reordenar los archivos multimedia de un producto.
/// El admin puede definir libremente el orden de visualización.
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

        foreach (var item in dto.Items)
        {
            var media = await _mediaRepository.GetByIdAsync(item.MediaId);

            if (media is null || media.ProductId != productId)
                throw new InvalidOperationException(
                    $"No existe un archivo multimedia con Id '{item.MediaId}' para este producto.");

            media.SetDisplayOrder(item.DisplayOrder);
        }

        await _mediaRepository.SaveChangesAsync();
    }
}