namespace PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

/// <summary>
/// Define el tipo de archivo multimedia asociado a un producto.
/// </summary>
public enum MediaType
{
    /// <summary>Imagen del producto (máximo 9 por producto).</summary>
    Image,

    /// <summary>Video del producto (máximo 1 por producto, opcional).</summary>
    Video
}