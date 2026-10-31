namespace PaginaVentasNet.Api.Modules.Catalog.Domain;

/// <summary>
/// Opción concreta de un producto que el cliente compra: tiene su propio SKU y su propio stock.
/// SizeId y ColorId son opcionales. Un producto sin talla ni color tiene una sola variante con ambos en null.
/// </summary>
public class ProductVariant
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int? SizeId { get; private set; }
    public int? ColorId { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public int Stock { get; private set; }
    public bool IsActive { get; private set; }

    public Size? Size { get; private set; }
    public Color? Color { get; private set; }

    private ProductVariant() { }

    public static ProductVariant Create(
        int productId,
        int? sizeId,
        int? colorId,
        string sku,
        int stock)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("El SKU de la variante es obligatorio.");

        if (stock < 0)
            throw new ArgumentException("El stock no puede ser negativo.");

        return new ProductVariant
        {
            ProductId = productId,
            SizeId = sizeId,
            ColorId = colorId,
            Sku = sku.Trim().ToUpper(),
            Stock = stock,
            IsActive = true
        };
    }

    public void ChangeSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("El SKU de la variante es obligatorio.");

        Sku = sku.Trim().ToUpper();
    }

    public void ChangeAttributes(int? sizeId, int? colorId)
    {
        SizeId = sizeId;
        ColorId = colorId;
    }

    public void SetStock(int stock)
    {
        if (stock < 0)
            throw new ArgumentException("El stock no puede ser negativo.");

        Stock = stock;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity > Stock)
            throw new InvalidOperationException("Stock insuficiente.");

        Stock -= quantity;
    }

    public void RestoreStock(int quantity)
    {
        Stock += quantity;
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("La variante ya está activa.");
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("La variante ya está desactivada.");
        IsActive = false;
    }
}