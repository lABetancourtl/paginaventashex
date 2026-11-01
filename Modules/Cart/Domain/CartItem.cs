using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Cart.Domain;

/// <summary>
/// Representa un producto dentro del carrito con su cantidad.
/// </summary>
public class CartItem
{
    public int Id { get; private set; }
    public int CartId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Product? Product { get; private set; }
    public int? VariantId { get; private set; }
    public ProductVariant? Variant { get; private set; }

    private CartItem() { }

    public static CartItem Create(int cartId, int productId, int quantity, int? variantId = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.");

        return new CartItem
        {
            CartId = cartId,
            ProductId = productId,
            VariantId = variantId,
            Quantity = quantity,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.");

        Quantity = quantity;
    }
}