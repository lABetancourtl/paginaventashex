using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Home.Domain;

/// <summary>
/// Un producto elegido por el admin para un estante de la página de inicio.
/// La clave es la combinación sección + producto: un producto no se repite en el mismo estante.
/// </summary>
public class HomeSectionProduct
{
    public int SectionId { get; private set; }
    public int ProductId { get; private set; }
    public Product? Product { get; private set; }
    public int DisplayOrder { get; private set; }

    private HomeSectionProduct() { }

    public static HomeSectionProduct Create(int sectionId, int productId, int displayOrder)
    {
        if (productId <= 0)
            throw new ArgumentException("El producto es obligatorio.");

        if (displayOrder < 1)
            throw new ArgumentException("El orden debe ser mayor a cero.");

        return new HomeSectionProduct
        {
            SectionId = sectionId,
            ProductId = productId,
            DisplayOrder = displayOrder
        };
    }

    public void SetDisplayOrder(int order)
    {
        if (order < 1)
            throw new ArgumentException("El orden debe ser mayor a cero.");

        DisplayOrder = order;
    }
}