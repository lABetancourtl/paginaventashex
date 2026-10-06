namespace PaginaVentasNet.Api.Modules.Orders.Domain;

/// <summary>
/// Representa un producto dentro de un pedido.
/// Los datos del producto se copian al momento de crear el pedido.
/// </summary>
public class OrderItem
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal ProductPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal Subtotal => ProductPrice * Quantity;

    private OrderItem() { }

    public static OrderItem Create(
        int orderId,
        int productId,
        string productName,
        decimal productPrice,
        int quantity)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("El nombre del producto es obligatorio.");

        if (productPrice <= 0)
            throw new ArgumentException("El precio del producto debe ser mayor a cero.");

        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.");

        return new OrderItem
        {
            OrderId = orderId,
            ProductId = productId,
            ProductName = productName,
            ProductPrice = productPrice,
            Quantity = quantity
        };
    }
}