using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Variants.UseCases;

/// <summary>
/// Edita el SKU, el stock y los atributos de una variante. Reemplaza la variante completa:
/// si no se envía sizeId o colorId, ese atributo queda en null. Los atributos solo se pueden
/// cambiar si la variante es la única del producto.
/// </summary>
public class UpdateProductVariantUseCase
{
    private readonly IProductRepository _products;
    private readonly IProductVariantRepository _variants;
    private readonly ISizeRepository _sizes;
    private readonly IColorRepository _colors;

    public UpdateProductVariantUseCase(
        IProductRepository products,
        IProductVariantRepository variants,
        ISizeRepository sizes,
        IColorRepository colors)
    {
        _products = products;
        _variants = variants;
        _sizes = sizes;
        _colors = colors;
    }

    public async Task ExecuteAsync(int productId, int variantId, UpdateProductVariantDto dto)
    {
        var product = await _products.GetEntityByIdAsync(productId);

        if (product is null)
            throw new AppException("PRODUCT_NOT_FOUND",
                $"No se encontró el producto con Id {productId}.", 404);

        var variant = await _variants.GetEntityByIdAsync(variantId);

        if (variant is null || variant.ProductId != productId)
            throw new AppException("VARIANT_NOT_FOUND",
                $"No se encontró la variante con Id {variantId} en este producto.", 404);

        var sizeChanged = variant.SizeId != dto.SizeId;
        var colorChanged = variant.ColorId != dto.ColorId;

        if (sizeChanged)
            await VariantRules.EnsureSizeAvailableAsync(_sizes, dto.SizeId);

        if (colorChanged)
            await VariantRules.EnsureColorAvailableAsync(_colors, dto.ColorId);

        if (sizeChanged || colorChanged)
        {
            var all = await _variants.GetByProductIdAsync(productId);

            if (all.Count > 1)
                throw new AppException("VARIANT_VALIDATION",
                    "Los atributos de una variante solo se pueden cambiar si es la única del producto. " +
                    "Desactívala y crea otra con los atributos nuevos.", 400);
        }

        var sku = dto.Sku.Trim().ToUpper();

        if (await _variants.ExistsBySkuAsync(sku, excludeId: variantId))
            throw new AppException("VARIANT_CONFLICT",
                $"Ya existe una variante con el SKU '{sku}'.", 409);

        try
        {
            variant.ChangeSku(sku);
            variant.SetStock(dto.Stock);
        }
        catch (ArgumentException ex)
        {
            throw new AppException("VARIANT_VALIDATION", ex.Message, 400);
        }

        variant.ChangeAttributes(dto.SizeId, dto.ColorId);

        await _variants.SaveChangesAsync();
    }
}