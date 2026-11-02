using PaginaVentasNet.Api.Modules.Cart.Application.Dtos;
using PaginaVentasNet.Api.Modules.Cart.Application.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Ports;

namespace PaginaVentasNet.Api.Modules.Cart.Application.UseCases;

/// <summary>
/// Caso de uso para actualizar la cantidad de un item del carrito.
/// La cantidad se valida contra el stock de la variante del item.
/// </summary>
public class UpdateCartItemUseCase
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductVariantRepository _variantRepository;

    public UpdateCartItemUseCase(
        ICartRepository cartRepository,
        IProductVariantRepository variantRepository)
    {
        _cartRepository = cartRepository;
        _variantRepository = variantRepository;
    }

    public async Task ExecuteAsync(int usuarioId, int itemId, UpdateCartItemDto dto)
    {
        var item = await _cartRepository.GetItemByIdAsync(itemId);
        var cart = await _cartRepository.GetByUsuarioIdAsync(usuarioId);

        if (item is null || cart is null || item.CartId != cart.Id)
            throw new InvalidOperationException(
                $"No existe el item con Id '{itemId}' en tu carrito.");

        var variant = await _variantRepository.GetEntityByIdAsync(item.VariantId);

        if (variant is null || !variant.IsActive)
            throw new InvalidOperationException(
                "Esta variante ya no está disponible. Elimina el item del carrito.");

        if (dto.Quantity > variant.Stock)
            throw new InvalidOperationException(
                $"No hay suficiente stock. Stock disponible: {variant.Stock}.");

        item.UpdateQuantity(dto.Quantity);
        await _cartRepository.SaveChangesAsync();
    }
}