namespace PaginaVentasNet.Api.Modules.Orders.Domain;

/// <summary>
/// Representa un producto dentro de un pedido.
/// Todo se copia al momento de crear el pedido: nombre, talla, color, SKU, precio base,
/// porcentaje de descuento y precio pagado. Así el historial no cambia si el producto cambia después.
/// </summary>
public class OrderItem
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }

    /// <summary>Variante comprada. Es null en los pedidos anteriores a las variantes.</summary>
    public int? VariantId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;
    public string? SizeName { get; private set; }
    public string? ColorName { get; private set; }
    public string? Sku { get; private set; }

    /// <summary>Precio sin descuento al momento de la compra.</summary>
    public decimal BasePrice { get; private set; }

    /// <summary>Porcentaje de descuento aplicado, o null si no había descuento.</summary>
    public int? DiscountPercent { get; private set; }

    /// <summary>Precio que realmente pagó el cliente por unidad.</summary>
    public decimal ProductPrice { get; private set; }

    public int Quantity { get; private set; }
    public decimal Subtotal => ProductPrice * Quantity;

    private OrderItem() { }

    public static OrderItem Create(
        int orderId,
        int productId,
        string productName,
        decimal productPrice,
        int quantity,
        int? variantId = null,
        string? sizeName = null,
        string? colorName = null,
        string? sku = null,
        decimal? basePrice = null,
        int? discountPercent = null)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("El nombre del producto es obligatorio.");

        if (productPrice <= 0)
            throw new ArgumentException("El precio del producto debe ser mayor a cero.");

        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.");

        if (discountPercent is < 1 or > 99)
            throw new ArgumentException("El descuento debe estar entre 1 y 99.");

        var effectiveBasePrice = basePrice ?? productPrice;

        if (effectiveBasePrice < productPrice)
            throw new ArgumentException("El precio base no puede ser menor al precio pagado.");

        return new OrderItem
        {
            OrderId = orderId,
            ProductId = productId,
            VariantId = variantId,
            ProductName = productName,
            SizeName = sizeName,
            ColorName = colorName,
            Sku = sku,
            BasePrice = effectiveBasePrice,
            DiscountPercent = discountPercent,
            ProductPrice = productPrice,
            Quantity = quantity
        };
    }
}