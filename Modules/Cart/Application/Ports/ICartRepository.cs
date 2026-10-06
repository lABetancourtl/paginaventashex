using PaginaVentasNet.Api.Modules.Cart.Application.Dtos;
using PaginaVentasNet.Api.Modules.Cart.Domain;

namespace PaginaVentasNet.Api.Modules.Cart.Application.Ports;

public interface ICartRepository
{
    Task<ShoppingCart?> GetByUsuarioIdAsync(int usuarioId);
    Task<CartResponseDto?> GetCartDtoByUsuarioIdAsync(int usuarioId);
    Task<CartItem?> GetItemByIdAsync(int itemId);
    Task<CartItem?> GetItemByProductIdAsync(int cartId, int productId);
    Task AddAsync(ShoppingCart cart);
    Task AddItemAsync(CartItem item);
    Task RemoveItemAsync(CartItem item);
    Task ClearItemsAsync(int cartId);
    Task SaveChangesAsync();
}