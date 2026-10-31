using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Variants.UseCases;

/// <summary>
/// Reglas que comparten crear, editar y reactivar una variante.
/// </summary>
internal static class VariantRules
{
    /// <summary>La talla, si se envía, debe existir y estar activa.</summary>
    public static async Task EnsureSizeAvailableAsync(ISizeRepository sizes, int? sizeId)
    {
        if (sizeId is null)
            return;

        var size = await sizes.GetEntityByIdAsync(sizeId.Value);

        if (size is null)
            throw new AppException("VARIANT_VALIDATION",
                $"No existe la talla con Id {sizeId}.", 400);

        if (!size.IsActive)
            throw new AppException("VARIANT_VALIDATION",
                $"La talla '{size.Name}' está desactivada.", 400);
    }

    /// <summary>El color, si se envía, debe existir y estar activo.</summary>
    public static async Task EnsureColorAvailableAsync(IColorRepository colors, int? colorId)
    {
        if (colorId is null)
            return;

        var color = await colors.GetEntityByIdAsync(colorId.Value);

        if (color is null)
            throw new AppException("VARIANT_VALIDATION",
                $"No existe el color con Id {colorId}.", 400);

        if (!color.IsActive)
            throw new AppException("VARIANT_VALIDATION",
                $"El color '{color.Name}' está desactivado.", 400);
    }

    /// <summary>
    /// Todas las variantes activas de un producto deben usar los mismos atributos:
    /// si una tiene talla, todas la tienen, y lo mismo con el color.
    /// </summary>
    public static void EnsureCoherent(IEnumerable<ProductVariant> others, int? sizeId, int? colorId)
    {
        var mismatch = others.FirstOrDefault(v =>
            v.IsActive
            && (v.SizeId.HasValue != sizeId.HasValue
                || v.ColorId.HasValue != colorId.HasValue));

        if (mismatch is null)
            return;

        throw new AppException("VARIANT_VALIDATION",
            $"Las variantes activas de este producto usan {Describe(mismatch.SizeId, mismatch.ColorId)} " +
            $"y esta usa {Describe(sizeId, colorId)}. Todas deben usar los mismos atributos. " +
            "Si el producto tiene una sola variante, edítala para cambiar sus atributos.", 400);
    }

    private static string Describe(int? sizeId, int? colorId)
    {
        if (sizeId.HasValue && colorId.HasValue) return "talla y color";
        if (sizeId.HasValue) return "solo talla";
        if (colorId.HasValue) return "solo color";
        return "ni talla ni color";
    }
}