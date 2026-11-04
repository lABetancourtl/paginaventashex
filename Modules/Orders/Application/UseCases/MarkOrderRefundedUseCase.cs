using PaginaVentasNet.Api.Modules.Orders.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Orders.Application.UseCases;

/// <summary>
/// Caso de uso para registrar que el administrador ya devolvió el dinero de un pedido cancelado.
/// No mueve dinero: la devolución se hace en Wompi o por fuera, y aquí solo se deja constancia.
/// </summary>
public class MarkOrderRefundedUseCase
{
    private readonly IOrderRepository _repository;

    public MarkOrderRefundedUseCase(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(int orderId)
    {
        var order = await _repository.GetEntityByIdAsync(orderId);

        if (order is null)
            throw new InvalidOperationException(
                $"No existe un pedido con Id '{orderId}'.");

        order.MarkRefunded();
        await _repository.SaveChangesAsync();
    }
}