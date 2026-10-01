using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.UseCases;

/// <summary>
/// Caso de uso para obtener todos los archivos multimedia de un producto dado su ID.
/// </summary>
public class GetProductMediaUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IProductMediaRepository _mediaRepository;

    public GetProductMediaUseCase(
        IProductRepository productRepository,
        IProductMediaRepository mediaRepository)
    {
        _productRepository = productRepository;
        _mediaRepository = mediaRepository;
    }

    public async Task<List<ProductMediaResponseDto>> ExecuteAsync(int productId)
    {
        var product = await _productRepository.GetEntityByIdAsync(productId);

        if (product is null)
            throw new InvalidOperationException(
                $"No existe un producto con Id '{productId}'.");

        return await _mediaRepository.GetAllByProductIdAsync(productId);
    }
}