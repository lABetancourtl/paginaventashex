using PaginaVentasNet.Api.Modules.Cart.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Cart.Application.UseCases;

/// <summary>
/// Caso de uso para vaciar el carrito completo del usuario.
/// </summary>
public class ClearCartUseCase
{
    private readonly ICartRepository _cartRepository;

    public ClearCartUseCase(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task ExecuteAsync(int usuarioId)
    {
        var cart = await _cartRepository.GetByUsuarioIdAsync(usuarioId);

        if (cart is null)
            throw new InvalidOperationException("No tienes un carrito activo.");

        await _cartRepository.ClearItemsAsync(cart.Id);
        await _cartRepository.SaveChangesAsync();
    }
}