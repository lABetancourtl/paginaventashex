using PaginaVentasNet.Api.Modules.Cart.Application.Dtos;
using PaginaVentasNet.Api.Modules.Cart.Application.Ports;
using PaginaVentasNet.Api.Modules.Cart.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Cart.Application.UseCases;

/// <summary>
/// Caso de uso para agregar un producto al carrito.
/// Si el producto ya existe en el carrito, actualiza la cantidad.
/// Si el usuario no tiene carrito, lo crea automáticamente.
/// </summary>
public class AddCartItemUseCase
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public AddCartItemUseCase(
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task ExecuteAsync(int usuarioId, AddCartItemDto dto)
    {
        var product = await _productRepository.GetEntityByIdAsync(dto.ProductId);

        if (product is null || !product.IsActive)
            throw new InvalidOperationException(
                $"El producto con Id '{dto.ProductId}' no existe o no está disponible.");

        if (dto.Quantity > product.Stock)
            throw new InvalidOperationException(
                $"No hay suficiente stock. Stock disponible: {product.Stock}.");

        var cart = await _cartRepository.GetByUsuarioIdAsync(usuarioId);

        if (cart is null)
        {
            cart = ShoppingCart.Create(usuarioId);
            await _cartRepository.AddAsync(cart);
            await _cartRepository.SaveChangesAsync();
        }

        var existingItem = await _cartRepository.GetItemByProductIdAsync(cart.Id, dto.ProductId);

        if (existingItem is not null)
        {
            var newQuantity = existingItem.Quantity + dto.Quantity;

            if (newQuantity > product.Stock)
                throw new InvalidOperationException(
                    $"No hay suficiente stock. Stock disponible: {product.Stock}.");

            existingItem.UpdateQuantity(newQuantity);
        }
        else
        {
            var item = CartItem.Create(cart.Id, dto.ProductId, dto.Quantity);
            await _cartRepository.AddItemAsync(item);
        }

        await _cartRepository.SaveChangesAsync();
    }
}