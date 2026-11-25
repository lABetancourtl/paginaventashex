using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Home.Domain;

/// <summary>
/// Un medio (imagen o video) de un banner. Puede enlazar a una categoría: al hacer clic,
/// la tienda lleva al catálogo con esa categoría seleccionada.
/// Si el banner tiene un solo medio, este puede llevar una imagen aparte para celular (formato 4:5).
/// </summary>
public class HomeSectionItem
{
    public int Id { get; private set; }
    public int SectionId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string PublicId { get; private set; } = string.Empty;
    public MediaType MediaType { get; private set; }

    /// <summary>Categoría a la que lleva el medio. Es null si el medio no es un enlace.</summary>
    public int? CategoryId { get; private set; }
    public Category? Category { get; private set; }

    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Imagen 4:5 que se muestra en celular en lugar de Url. Es null si no tiene.
    /// Solo se sirve a la tienda cuando el banner tiene un único medio.
    /// </summary>
    public string? MobileUrl { get; private set; }
    public string? MobilePublicId { get; private set; }

    private HomeSectionItem() { }

    public static HomeSectionItem Create(
        int sectionId,
        string url,
        string publicId,
        MediaType mediaType,
        int? categoryId,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("La URL del medio es obligatoria.");

        if (string.IsNullOrWhiteSpace(publicId))
            throw new ArgumentException("El identificador del medio es obligatorio.");

        if (!Enum.IsDefined(mediaType))
            throw new ArgumentException("El tipo de medio no es válido.");

        if (displayOrder < 1)
            throw new ArgumentException("El orden debe ser mayor a cero.");

        return new HomeSectionItem
        {
            SectionId = sectionId,
            Url = url,
            PublicId = publicId,
            MediaType = mediaType,
            CategoryId = categoryId,
            DisplayOrder = displayOrder
        };
    }

    public void SetCategory(int? categoryId)
    {
        CategoryId = categoryId;
    }

    public void SetDisplayOrder(int order)
    {
        if (order < 1)
            throw new ArgumentException("El orden debe ser mayor a cero.");

        DisplayOrder = order;
    }

    public void SetMobileImage(string url, string publicId)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("La URL de la imagen para celular es obligatoria.");

        if (string.IsNullOrWhiteSpace(publicId))
            throw new ArgumentException("El identificador de la imagen para celular es obligatorio.");

        MobileUrl = url;
        MobilePublicId = publicId;
    }

    public void ClearMobileImage()
    {
        MobileUrl = null;
        MobilePublicId = null;
    }
}
