using PaginaVentasNet.Api.Modules.Cart.Application.Dtos;
using PaginaVentasNet.Api.Modules.Cart.Application.Ports;
using PaginaVentasNet.Api.Modules.Cart.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Ports;

namespace PaginaVentasNet.Api.Modules.Cart.Application.UseCases;

/// <summary>
/// Caso de uso para agregar una variante de producto al carrito.
/// La variante debe estar activa, y su producto, su subcategoría y su categoría principal también.
/// Si la variante ya está en el carrito, suma la cantidad.
/// Si el usuario no tiene carrito, lo crea automáticamente.
/// </summary>
public class AddCartItemUseCase
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductVariantRepository _variantRepository;
    private readonly ICategoryRepository _categoryRepository;

    public AddCartItemUseCase(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IProductVariantRepository variantRepository,
        ICategoryRepository categoryRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task ExecuteAsync(int usuarioId, AddCartItemDto dto)
    {
        var unavailable = $"La variante con Id '{dto.VariantId}' no existe o no está disponible.";

        var variant = await _variantRepository.GetEntityByIdAsync(dto.VariantId);

        if (variant is null || !variant.IsActive)
            throw new InvalidOperationException(unavailable);

        var product = await _productRepository.GetEntityByIdAsync(variant.ProductId);

        if (product is null || !product.IsActive)
            throw new InvalidOperationException(unavailable);

        if (!await IsCategoryVisibleAsync(product.CategoryId))
            throw new InvalidOperationException(unavailable);

        if (dto.Quantity > variant.Stock)
            throw new InvalidOperationException(
                $"No hay suficiente stock. Stock disponible: {variant.Stock}.");

        var cart = await _cartRepository.GetByUsuarioIdAsync(usuarioId);

        if (cart is null)
        {
            cart = ShoppingCart.Create(usuarioId);
            await _cartRepository.AddAsync(cart);
            await _cartRepository.SaveChangesAsync();
        }

        var existingItem = await _cartRepository.GetItemByVariantIdAsync(cart.Id, variant.Id);

        if (existingItem is not null)
        {
            var newQuantity = existingItem.Quantity + dto.Quantity;

            if (newQuantity > variant.Stock)
                throw new InvalidOperationException(
                    $"No hay suficiente stock. Stock disponible: {variant.Stock}.");

            existingItem.UpdateQuantity(newQuantity);
        }
        else
        {
            var item = CartItem.Create(cart.Id, product.Id, dto.Quantity, variant.Id);
            await _cartRepository.AddItemAsync(item);
        }

        await _cartRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Misma regla del catálogo: la subcategoría y su categoría principal deben estar activas.
    /// </summary>
    private async Task<bool> IsCategoryVisibleAsync(int categoryId)
    {
        var category = await _categoryRepository.GetEntityByIdAsync(categoryId);

        if (category is null || !category.IsActive)
            return false;

        if (category.ParentCategoryId is null)
            return true;

        var parent = await _categoryRepository.GetEntityByIdAsync(category.ParentCategoryId.Value);

        return parent is not null && parent.IsActive;
    }
}