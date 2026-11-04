using System.Linq.Expressions;
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

    /// <summary>
    /// Única proyección de pedido a su respuesta. Las cuatro consultas la comparten,
    /// así un campo nuevo se agrega en un solo lugar.
    /// </summary>
    private static readonly Expression<Func<Order, OrderResponseDto>> ToResponseDto = o => new OrderResponseDto
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
        RefundStatus = o.RefundStatus,
        RefundedAtUtc = o.RefundedAtUtc,
        Items = o.Items.Select(i => new OrderItemResponseDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            VariantId = i.VariantId,
            ProductName = i.ProductName,
            SizeName = i.SizeName,
            ColorName = i.ColorName,
            Sku = i.Sku,
            BasePrice = i.BasePrice,
            DiscountPercent = i.DiscountPercent,
            ProductPrice = i.ProductPrice,
            Quantity = i.Quantity,
            Subtotal = i.ProductPrice * i.Quantity
        }).ToList()
    };

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
            .Select(ToResponseDto)
            .FirstOrDefaultAsync();
    }

    public async Task<List<OrderResponseDto>> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _context.Orders
            .Where(o => o.UsuarioId == usuarioId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(ToResponseDto)
            .ToListAsync();
    }

    public async Task<List<OrderResponseDto>> GetAllAsync()
    {
        return await _context.Orders
            .Where(o => o.Status == OrderStatus.Confirmado ||
                        o.Status == OrderStatus.Enviado)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(ToResponseDto)
            .ToListAsync();
    }

    public async Task<List<OrderResponseDto>> GetByRefundStatusAsync(RefundStatus status)
    {
        var query = _context.Orders.Where(o => o.RefundStatus == status);

        // Pendientes: las más antiguas primero. Completadas: las más recientes primero.
        query = status == RefundStatus.Completed
            ? query.OrderByDescending(o => o.RefundedAtUtc)
            : query.OrderBy(o => o.CreatedAtUtc);

        return await query
            .Select(ToResponseDto)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}