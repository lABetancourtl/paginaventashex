using PaginaVentasNet.Api.Modules.Cart.Application.Ports;
using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Ports;
using PaginaVentasNet.Api.Modules.Orders.Application.Dtos;
using PaginaVentasNet.Api.Modules.Orders.Application.Ports;
using PaginaVentasNet.Api.Modules.Orders.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Orders.Application.UseCases;

/// <summary>
/// Caso de uso para crear un pedido desde el carrito del usuario.
/// Valida stock, copia datos de dirección y producto, descuenta stock y vacía el carrito.
/// </summary>
public class CreateOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IDireccionRepository _direccionRepository;
    private readonly IProductRepository _productRepository;

    public CreateOrderUseCase(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IDireccionRepository direccionRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _direccionRepository = direccionRepository;
        _productRepository = productRepository;
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

        foreach (var item in cart.Items)
        {
            var product = await _productRepository.GetEntityByIdAsync(item.ProductId);

            if (product is null || !product.IsActive)
                throw new InvalidOperationException(
                    $"El producto '{item.Product?.Name}' ya no está disponible.");

            if (item.Quantity > product.Stock)
                throw new InvalidOperationException(
                    $"Stock insuficiente para '{product.Name}'. Stock disponible: {product.Stock}.");
        }

        var order = Order.Create(
            usuarioId,
            departamento,
            municipio,
            direccion.DireccionTexto,
            direccion.NombreQuienRecibe,
            direccion.InformacionAdicional,
            direccion.Barrio);

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        decimal total = 0;

        foreach (var item in cart.Items)
        {
            var product = await _productRepository.GetEntityByIdAsync(item.ProductId);

            var orderItem = OrderItem.Create(
                order.Id,
                product!.Id,
                product.Name,
                product.Price,
                item.Quantity);

            order.Items.Add(orderItem);

            product.ReduceStock(item.Quantity);
            total += product.Price * item.Quantity;
        }

        order.SetTotal(total);
        await _cartRepository.ClearItemsAsync(cart.Id);
        await _orderRepository.SaveChangesAsync();

        return order.Id;
    }
}