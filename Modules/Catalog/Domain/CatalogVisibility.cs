using System.Linq.Expressions;

namespace PaginaVentasNet.Api.Modules.Catalog.Domain;

/// <summary>
/// Única definición de lo que la tienda muestra al público.
/// Una categoría es visible si está activa y su categoría principal también (o si es una raíz).
/// Un producto es visible si está activo y su categoría es visible.
/// Cualquier consulta o comprobación de visibilidad debe pasar por aquí: así, si la regla cambia,
/// cambia en un solo lugar.
/// </summary>
public static class CatalogVisibility
{
    public static readonly Expression<Func<Category, bool>> CategoryIsVisible =
        c => c.IsActive
             && (c.ParentCategory == null || c.ParentCategory.IsActive);

    public static readonly Expression<Func<Product, bool>> ProductIsVisible =
        p => p.IsActive
             && p.Category!.IsActive
             && (p.Category.ParentCategory == null || p.Category.ParentCategory.IsActive);

    /// <summary>Deja solo las categorías visibles en la tienda.</summary>
    public static IQueryable<Category> WhereVisible(this IQueryable<Category> query)
        => query.Where(CategoryIsVisible);

    /// <summary>Deja solo los productos visibles en la tienda.</summary>
    public static IQueryable<Product> WhereVisible(this IQueryable<Product> query)
        => query.Where(ProductIsVisible);
}