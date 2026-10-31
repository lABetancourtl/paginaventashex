using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Variants.UseCases;

/// <summary>
/// Lista todas las variantes de un producto, activas o no, para el panel admin.
/// </summary>
public class GetProductVariantsUseCase
{
    private readonly IProductRepository _products;
    private readonly IProductVariantRepository _variants;

    public GetProductVariantsUseCase(
        IProductRepository products,
        IProductVariantRepository variants)
    {
        _products = products;
        _variants = variants;
    }

    public async Task<List<ProductVariantResponseDto>> ExecuteAsync(int productId)
    {
        var product = await _products.GetEntityByIdAsync(productId);

        if (product is null)
            throw new AppException("PRODUCT_NOT_FOUND",
                $"No se encontró el producto con Id {productId}.", 404);

        return await _variants.GetAllDtosByProductIdAsync(productId);
    }
}