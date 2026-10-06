using PaginaVentasNet.Api.Modules.Cart.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Cart.Application.UseCases;

/// <summary>
/// Caso de uso para eliminar un item del carrito.
/// </summary>
public class RemoveCartItemUseCase
{
    private readonly ICartRepository _cartRepository;

    public RemoveCartItemUseCase(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task ExecuteAsync(int usuarioId, int itemId)
    {
        var cart = await _cartRepository.GetByUsuarioIdAsync(usuarioId);
        var item = await _cartRepository.GetItemByIdAsync(itemId);

        if (item is null || cart is null || item.CartId != cart.Id)
            throw new InvalidOperationException(
                $"No existe el item con Id '{itemId}' en tu carrito.");

        await _cartRepository.RemoveItemAsync(item);
        await _cartRepository.SaveChangesAsync();
    }
}