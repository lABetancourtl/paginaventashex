using PaginaVentasNet.Api.Modules.Orders.Application.Dtos;
using PaginaVentasNet.Api.Modules.Orders.Domain;

namespace PaginaVentasNet.Api.Modules.Orders.Application.Ports;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> GetEntityByIdAsync(int id);
    Task<OrderResponseDto?> GetByIdAsync(int id, int usuarioId);
    Task<List<OrderResponseDto>> GetByUsuarioIdAsync(int usuarioId);
    Task<List<OrderResponseDto>> GetAllAsync();
    Task SaveChangesAsync();
}