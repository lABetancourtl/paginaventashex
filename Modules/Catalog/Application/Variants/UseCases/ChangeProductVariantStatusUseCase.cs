using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Variants.UseCases;

/// <summary>
/// Activa o desactiva una variante. Al reactivar, debe ser coherente con las demás variantes activas.
/// </summary>
public class ChangeProductVariantStatusUseCase
{
    private readonly IProductRepository _products;
    private readonly IProductVariantRepository _variants;

    public ChangeProductVariantStatusUseCase(
        IProductRepository products,
        IProductVariantRepository variants)
    {
        _products = products;
        _variants = variants;
    }

    public async Task ActivateAsync(int productId, int variantId)
    {
        var variant = await GetOrThrowAsync(productId, variantId);

        if (variant.IsActive)
            throw new AppException("VARIANT_INVALID_STATE", "La variante ya está activa.", 400);

        var others = await _variants.GetByProductIdAsync(productId);
        VariantRules.EnsureCoherent(others, variant.SizeId, variant.ColorId);

        variant.Activate();
        await _variants.SaveChangesAsync();
    }

    public async Task DeactivateAsync(int productId, int variantId)
    {
        var variant = await GetOrThrowAsync(productId, variantId);

        if (!variant.IsActive)
            throw new AppException("VARIANT_INVALID_STATE", "La variante ya está desactivada.", 400);

        variant.Deactivate();
        await _variants.SaveChangesAsync();
    }

    private async Task<ProductVariant> GetOrThrowAsync(int productId, int variantId)
    {
        var product = await _products.GetEntityByIdAsync(productId);

        if (product is null)
            throw new AppException("PRODUCT_NOT_FOUND",
                $"No se encontró el producto con Id {productId}.", 404);

        var variant = await _variants.GetEntityByIdAsync(variantId);

        if (variant is null || variant.ProductId != productId)
            throw new AppException("VARIANT_NOT_FOUND",
                $"No se encontró la variante con Id {variantId} en este producto.", 404);

        return variant;
    }
}