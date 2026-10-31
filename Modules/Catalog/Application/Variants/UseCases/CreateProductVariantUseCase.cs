using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Variants.UseCases;

/// <summary>
/// Agrega una variante a un producto. Talla y color son opcionales, pero deben ser
/// coherentes con las demás variantes activas, y la combinación no se repite.
/// </summary>
public class CreateProductVariantUseCase
{
    private readonly IProductRepository _products;
    private readonly IProductVariantRepository _variants;
    private readonly ISizeRepository _sizes;
    private readonly IColorRepository _colors;

    public CreateProductVariantUseCase(
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

    public async Task<int> ExecuteAsync(int productId, CreateProductVariantDto dto)
    {
        var product = await _products.GetEntityByIdAsync(productId);

        if (product is null)
            throw new AppException("PRODUCT_NOT_FOUND",
                $"No se encontró el producto con Id {productId}.", 404);

        await VariantRules.EnsureSizeAvailableAsync(_sizes, dto.SizeId);
        await VariantRules.EnsureColorAvailableAsync(_colors, dto.ColorId);

        var existing = await _variants.GetByProductIdAsync(productId);

        VariantRules.EnsureCoherent(existing, dto.SizeId, dto.ColorId);

        if (existing.Any(v => v.SizeId == dto.SizeId && v.ColorId == dto.ColorId))
            throw new AppException("VARIANT_CONFLICT",
                "Este producto ya tiene una variante con esa talla y ese color. " +
                "Si no la ves activa, puede estar desactivada: reactívala en lugar de crear otra.", 409);

        var sku = dto.Sku.Trim().ToUpper();

        if (await _variants.ExistsBySkuAsync(sku))
            throw new AppException("VARIANT_CONFLICT",
                $"Ya existe una variante con el SKU '{sku}'.", 409);

        ProductVariant variant;

        try
        {
            variant = ProductVariant.Create(productId, dto.SizeId, dto.ColorId, sku, dto.Stock);
        }
        catch (ArgumentException ex)
        {
            throw new AppException("VARIANT_VALIDATION", ex.Message, 400);
        }

        await _variants.AddAsync(variant);
        await _variants.SaveChangesAsync();

        return variant.Id;
    }
}