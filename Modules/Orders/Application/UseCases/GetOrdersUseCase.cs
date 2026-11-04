using PaginaVentasNet.Api.Modules.Orders.Application.Dtos;
using PaginaVentasNet.Api.Modules.Orders.Application.Ports;
using PaginaVentasNet.Api.Modules.Orders.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Orders.Application.UseCases;

/// <summary>
/// Caso de uso para obtener los pedidos del usuario autenticado
/// o todos los pedidos si es admin.
/// </summary>
public class GetOrdersUseCase
{
    private readonly IOrderRepository _repository;

    public GetOrdersUseCase(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OrderResponseDto>> ExecuteAsync(int usuarioId)
    {
        return await _repository.GetByUsuarioIdAsync(usuarioId);
    }

    public async Task<OrderResponseDto?> ExecuteAsync(int orderId, int usuarioId)
    {
        return await _repository.GetByIdAsync(orderId, usuarioId);
    }

    public async Task<List<OrderResponseDto>> ExecuteAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<List<OrderResponseDto>> ExecuteByRefundStatusAsync(RefundStatus status)
    {
        if (!Enum.IsDefined(status) || status == RefundStatus.NotRequired)
            throw new InvalidOperationException(
                "Solo se pueden consultar devoluciones Pending o Completed.");

        return await _repository.GetByRefundStatusAsync(status);
    }
}