namespace PaginaVentasNet.Api.Modules.Cart.Application.Dtos;

public class CartResponseDto
{
    public int Id { get; set; }
    public List<CartItemResponseDto> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.Subtotal);
}

public class CartItemResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => ProductPrice * Quantity;
}