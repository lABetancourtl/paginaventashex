using PaginaVentasNet.Api.Modules.Cart.Application.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Ports;
using PaginaVentasNet.Api.Modules.Orders.Application.Dtos;
using PaginaVentasNet.Api.Modules.Orders.Application.Ports;
using PaginaVentasNet.Api.Modules.Orders.Domain;

namespace PaginaVentasNet.Api.Modules.Orders.Application.UseCases;

/// <summary>
/// Caso de uso para crear un pedido desde el carrito del usuario.
/// Valida cada variante (activa, con su producto y categorías visibles, y con stock suficiente),
/// copia al pedido los datos de la compra, descuenta el stock de las variantes y vacía el carrito.
/// Todo se guarda en una sola transacción: si algo falla, no se escribe nada.
/// </summary>
public class CreateOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IDireccionRepository _direccionRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductVariantRepository _variantRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateOrderUseCase(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IDireccionRepository direccionRepository,
        IProductRepository productRepository,
        IProductVariantRepository variantRepository,
        ICategoryRepository categoryRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _direccionRepository = direccionRepository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<int> ExecuteAsync(int usuarioId, CreateOrderDto dto)
    {
        var cart = await _cartRepository.GetByUsuarioIdAsync(usuarioId);

        if (cart is null || !cart.Items.Any())
            throw new InvalidOperationException(
                "El carrito está vacío. Agrega productos antes de crear un pedido.");

        var direccion = await _direccionRepository.GetEntityByIdAsync(dto.DireccionId);

        if (direccion is null || direccion.UsuarioId != usuarioId)
            throw new InvalidOperationException(
                "La dirección seleccionada no existe o no pertenece a tu cuenta.");

        var departamento = await _direccionRepository.GetDepartamentoNombreAsync(direccion.DepartamentoCodigo);
        var municipio = await _direccionRepository.GetMunicipioNombreAsync(direccion.MunicipioCodigo);

        var variants = await _variantRepository.GetByIdsWithAttributesAsync(
            cart.Items.Select(i => i.VariantId));
        var variantsById = variants.ToDictionary(v => v.Id);

        var order = Order.Create(
            usuarioId,
            departamento,
            municipio,
            direccion.DireccionTexto,
            direccion.NombreQuienRecibe,
            direccion.InformacionAdicional,
            direccion.Barrio);

        decimal total = 0;

        foreach (var item in cart.Items)
        {
            if (!variantsById.TryGetValue(item.VariantId, out var variant))
                throw new InvalidOperationException(
                    "Un producto de tu carrito ya no está disponible. Elimínalo del carrito e intenta de nuevo.");

            var product = await _productRepository.GetEntityByIdAsync(variant.ProductId);

            if (product is null)
                throw new InvalidOperationException(
                    "Un producto de tu carrito ya no está disponible. Elimínalo del carrito e intenta de nuevo.");

            var label = Describe(product.Name, variant);

            if (!variant.IsActive || !product.IsActive || !await IsCategoryVisibleAsync(product.CategoryId))
                throw new InvalidOperationException(
                    $"'{label}' ya no está disponible. Elimínalo del carrito e intenta de nuevo.");

            if (item.Quantity > variant.Stock)
                throw new InvalidOperationException(
                    $"Stock insuficiente para '{label}'. Stock disponible: {variant.Stock}.");

            order.Items.Add(OrderItem.Create(
                orderId: 0,
                productId: product.Id,
                productName: product.Name,
                productPrice: product.FinalPrice,
                quantity: item.Quantity,
                variantId: variant.Id,
                sizeName: variant.Size?.Name,
                colorName: variant.Color?.Name,
                sku: variant.Sku,
                basePrice: product.Price,
                discountPercent: product.DiscountPercent));

            variant.ReduceStock(item.Quantity);
            total += product.FinalPrice * item.Quantity;
        }

        order.SetTotal(total);

        await _orderRepository.AddAsync(order);
        await _cartRepository.ClearItemsAsync(cart.Id);
        await _orderRepository.SaveChangesAsync();

        return order.Id;
    }

    /// <summary>
    /// Misma regla del catálogo y del carrito: la subcategoría y su categoría principal deben estar activas.
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

    private static string Describe(string productName, ProductVariant variant)
    {
        var attributes = new[] { variant.Size?.Name, variant.Color?.Name }
            .Where(a => !string.IsNullOrEmpty(a))
            .ToList();

        return attributes.Count > 0
            ? $"{productName} ({string.Join(", ", attributes)})"
            : productName;
    }
}