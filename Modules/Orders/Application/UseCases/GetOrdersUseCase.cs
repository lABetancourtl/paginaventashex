using PaginaVentasNet.Api.Modules.Orders.Application.Dtos;
using PaginaVentasNet.Api.Modules.Orders.Application.Ports;

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
}