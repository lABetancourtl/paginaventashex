using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Ports;
using PaginaVentasNet.Api.Modules.Orders.Application.Ports;
using PaginaVentasNet.Api.Modules.Orders.Domain;

namespace PaginaVentasNet.Api.Modules.Orders.Application.UseCases;

/// <summary>
/// Caso de uso para cancelar un pedido, por el cliente dueño o por un administrador.
/// Restaura el stock de las variantes compradas y, si el pedido ya estaba pagado,
/// lo deja con la devolución pendiente. Los items de pedidos anteriores a las variantes
/// no tienen variante asociada y no restauran stock.
/// </summary>
public class CancelOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductVariantRepository _variantRepository;

    public CancelOrderUseCase(
        IOrderRepository orderRepository,
        IProductVariantRepository variantRepository)
    {
        _orderRepository = orderRepository;
        _variantRepository = variantRepository;
    }

    /// <summary>El cliente cancela un pedido propio.</summary>
    public async Task ExecuteAsync(int orderId, int usuarioId)
    {
        var order = await _orderRepository.GetEntityByIdAsync(orderId);

        if (order is null || order.UsuarioId != usuarioId)
            throw new InvalidOperationException(
                $"No existe un pedido con Id '{orderId}' en tu cuenta.");

        await CancelAsync(order);
    }

    /// <summary>El administrador cancela el pedido de cualquier cliente.</summary>
    public async Task ExecuteAsAdminAsync(int orderId)
    {
        var order = await _orderRepository.GetEntityByIdAsync(orderId);

        if (order is null)
            throw new InvalidOperationException(
                $"No existe un pedido con Id '{orderId}'.");

        await CancelAsync(order);
    }

    private async Task CancelAsync(Order order)
    {
        order.Cancel();

        foreach (var item in order.Items.Where(i => i.VariantId.HasValue))
        {
            var variant = await _variantRepository.GetEntityByIdAsync(item.VariantId!.Value);
            variant?.RestoreStock(item.Quantity);
        }

        await _orderRepository.SaveChangesAsync();
    }
}