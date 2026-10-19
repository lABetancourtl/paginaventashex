using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Domain;

/// <summary>
/// Representa un archivo multimedia (imagen o video) asociado a un producto.
/// </summary>
public class ProductMedia
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string PublicId { get; private set; } = string.Empty;
    public MediaType MediaType { get; private set; }
    public DisplayRole DisplayRole { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private ProductMedia() { }

    public static ProductMedia Create(
        int productId,
        string url,
        string publicId,
        MediaType mediaType,
        int displayOrder,
        DisplayRole displayRole = DisplayRole.None)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("La URL del archivo es obligatoria.");

        if (string.IsNullOrWhiteSpace(publicId))
            throw new ArgumentException("El PublicId de Cloudinary es obligatorio.");

        return new ProductMedia
        {
            ProductId = productId,
            Url = url,
            PublicId = publicId,
            MediaType = mediaType,
            DisplayOrder = displayOrder,
            DisplayRole = displayRole,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void SetDisplayRole(DisplayRole role)
    {
        DisplayRole = role;
    }

    public void SetDisplayOrder(int order)
    {
        if (order < 1)
            throw new ArgumentException("El orden debe ser mayor a cero.");
        DisplayOrder = order;
    }
}
