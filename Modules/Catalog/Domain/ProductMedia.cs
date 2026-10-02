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
    public int DisplayOrder { get; private set; }
    public bool IsMain { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private ProductMedia() { }

    public static ProductMedia Create(
        int productId,
        string url,
        string publicId,
        MediaType mediaType,
        int displayOrder,
        bool isMain = false)
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
            IsMain = isMain,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void SetAsMain()
    {
        IsMain = true;
    }

    public void RemoveFromMain()
    {
        IsMain = false;
    }
}
