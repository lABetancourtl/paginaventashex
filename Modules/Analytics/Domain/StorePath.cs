namespace PaginaVentasNet.Api.Modules.Analytics.Domain;

/// <summary>
/// Interpreta las rutas públicas de la tienda que registra Umami.
/// Deben coincidir con las rutas del frontend: /producto/:id y /categoria/:slug.
/// </summary>
public static class StorePath
{
    private const string ProductPrefix = "/producto/";
    private const string CategoryPrefix = "/categoria/";

    public static bool TryGetProductId(string path, out int productId)
    {
        productId = 0;
        var segment = GetSegment(path, ProductPrefix);
        return segment is not null && int.TryParse(segment, out productId) && productId > 0;
    }

    public static bool TryGetCategorySlug(string path, out string slug)
    {
        slug = GetSegment(path, CategoryPrefix)?.ToLowerInvariant() ?? string.Empty;
        return slug.Length > 0;
    }

    /// <summary>Primer segmento después del prefijo, sin query string, hash ni barra final.</summary>
    private static string? GetSegment(string path, string prefix)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;

        // Umami puede devolver la URL completa o solo la ruta.
        // Ojo: en Linux "/producto/1" también es un Uri absoluto (file://), por eso se exige http(s)
        if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            && Uri.TryCreate(path, UriKind.Absolute, out var uri))
            path = uri.AbsolutePath;

        var cut = path.IndexOfAny(['?', '#']);
        if (cut >= 0)
            path = path[..cut];

        if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return null;

        var rest = path[prefix.Length..].Trim('/');
        var slash = rest.IndexOf('/');
        if (slash >= 0)
            rest = rest[..slash];

        return rest.Length == 0 ? null : Uri.UnescapeDataString(rest);
    }
}
