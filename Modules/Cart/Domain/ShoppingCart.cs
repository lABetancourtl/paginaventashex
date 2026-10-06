namespace PaginaVentasNet.Api.Modules.Cart.Domain;

/// <summary>
/// Representa el carrito de compras de un usuario.
/// Cada usuario tiene un solo carrito.
/// </summary>
public class ShoppingCart
{
    public int Id { get; private set; }
    public int UsuarioId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public ICollection<CartItem> Items { get; private set; } = new List<CartItem>();

    private ShoppingCart() { }

    public static ShoppingCart Create(int usuarioId)
    {
        return new ShoppingCart
        {
            UsuarioId = usuarioId,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}