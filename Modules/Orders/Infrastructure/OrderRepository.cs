using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Orders.Application.Dtos;
using PaginaVentasNet.Api.Modules.Orders.Application.Ports;
using PaginaVentasNet.Api.Modules.Orders.Domain;
using PaginaVentasNet.Api.Modules.Orders.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Orders.Infrastructure;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task<Order?> GetEntityByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id, int usuarioId)
    {
        return await _context.Orders
            .Where(o => o.Id == id && o.UsuarioId == usuarioId)
            .Select(o => new OrderResponseDto
            {
                Id = o.Id,
                Status = o.Status,
                Total = o.Total,
                DepartamentoNombre = o.DepartamentoNombre,
                MunicipioNombre = o.MunicipioNombre,
                DireccionTexto = o.DireccionTexto,
                InformacionAdicional = o.InformacionAdicional,
                Barrio = o.Barrio,
                NombreQuienRecibe = o.NombreQuienRecibe,
                CreatedAtUtc = o.CreatedAtUtc,
                Items = o.Items.Select(i => new OrderItemResponseDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductPrice = i.ProductPrice,
                    Quantity = i.Quantity,
                    Subtotal = i.Subtotal
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<OrderResponseDto>> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _context.Orders
            .Where(o => o.UsuarioId == usuarioId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(o => new OrderResponseDto
            {
                Id = o.Id,
                Status = o.Status,
                Total = o.Total,
                DepartamentoNombre = o.DepartamentoNombre,
                MunicipioNombre = o.MunicipioNombre,
                DireccionTexto = o.DireccionTexto,
                InformacionAdicional = o.InformacionAdicional,
                Barrio = o.Barrio,
                NombreQuienRecibe = o.NombreQuienRecibe,
                CreatedAtUtc = o.CreatedAtUtc,
                Items = o.Items.Select(i => new OrderItemResponseDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductPrice = i.ProductPrice,
                    Quantity = i.Quantity,
                    Subtotal = i.Subtotal
                }).ToList()
            })
            .ToListAsync();
    }

    
    public async Task<List<OrderResponseDto>> GetAllAsync()
    {
        return await _context.Orders
            .Where(o => o.Status == OrderStatus.Confirmado || 
                        o.Status == OrderStatus.Enviado)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(o => new OrderResponseDto
            {
                Id = o.Id,
                Status = o.Status,
                Total = o.Total,
                DepartamentoNombre = o.DepartamentoNombre,
                MunicipioNombre = o.MunicipioNombre,
                DireccionTexto = o.DireccionTexto,
                InformacionAdicional = o.InformacionAdicional,
                Barrio = o.Barrio,
                NombreQuienRecibe = o.NombreQuienRecibe,
                CreatedAtUtc = o.CreatedAtUtc,
                Items = o.Items.Select(i => new OrderItemResponseDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductPrice = i.ProductPrice,
                    Quantity = i.Quantity,
                    Subtotal = i.Subtotal
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}