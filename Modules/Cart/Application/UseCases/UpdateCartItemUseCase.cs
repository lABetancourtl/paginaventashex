using PaginaVentasNet.Api.Modules.Cart.Application.Dtos;
using PaginaVentasNet.Api.Modules.Cart.Application.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Cart.Application.UseCases;

/// <summary>
/// Caso de uso para actualizar la cantidad de un item del carrito.
/// </summary>
public class UpdateCartItemUseCase
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public UpdateCartItemUseCase(
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task ExecuteAsync(int usuarioId, int itemId, UpdateCartItemDto dto)
    {
        var item = await _cartRepository.GetItemByIdAsync(itemId);
        var cart = await _cartRepository.GetByUsuarioIdAsync(usuarioId);

        if (item is null || cart is null || item.CartId != cart.Id)
            throw new InvalidOperationException(
                $"No existe el item con Id '{itemId}' en tu carrito.");

        var product = await _productRepository.GetEntityByIdAsync(item.ProductId);

        if (dto.Quantity > product!.Stock)
            throw new InvalidOperationException(
                $"No hay suficiente stock. Stock disponible: {product.Stock}.");

        item.UpdateQuantity(dto.Quantity);
        await _cartRepository.SaveChangesAsync();
    }
}