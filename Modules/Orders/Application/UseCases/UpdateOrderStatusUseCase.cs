using PaginaVentasNet.Api.Modules.Orders.Application.Dtos;
using PaginaVentasNet.Api.Modules.Orders.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Orders.Application.UseCases;

/// <summary>
/// Caso de uso para actualizar el estado de un pedido.
/// Solo puede ser ejecutado por un administrador.
/// </summary>
public class UpdateOrderStatusUseCase
{
    private readonly IOrderRepository _repository;

    public UpdateOrderStatusUseCase(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(int orderId, UpdateOrderStatusDto dto)
    {
        var order = await _repository.GetEntityByIdAsync(orderId);

        if (order is null)
            throw new InvalidOperationException(
                $"No existe un pedido con Id '{orderId}'.");

        order.UpdateStatus(dto.Status);
        await _repository.SaveChangesAsync();
    }
}