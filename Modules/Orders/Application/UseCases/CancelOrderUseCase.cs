using PaginaVentasNet.Api.Modules.Orders.Application.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Orders.Application.UseCases;

/// <summary>
/// Caso de uso para cancelar un pedido.
/// Restaura el stock de los productos al cancelar.
/// </summary>
public class CancelOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public CancelOrderUseCase(
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task ExecuteAsync(int orderId, int usuarioId)
    {
        var order = await _orderRepository.GetEntityByIdAsync(orderId);

        if (order is null || order.UsuarioId != usuarioId)
            throw new InvalidOperationException(
                $"No existe un pedido con Id '{orderId}' en tu cuenta.");

        order.Cancel();

        foreach (var item in order.Items)
        {
            var product = await _productRepository.GetEntityByIdAsync(item.ProductId);
            product?.RestoreStock(item.Quantity);
        }

        await _orderRepository.SaveChangesAsync();
    }
}