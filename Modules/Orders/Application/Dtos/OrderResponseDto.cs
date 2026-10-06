using PaginaVentasNet.Api.Modules.Orders.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Orders.Application.Dtos;

public class OrderResponseDto
{
    public int Id { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }
    public string DepartamentoNombre { get; set; } = string.Empty;
    public string MunicipioNombre { get; set; } = string.Empty;
    public string DireccionTexto { get; set; } = string.Empty;
    public string? InformacionAdicional { get; set; }
    public string? Barrio { get; set; }
    public string NombreQuienRecibe { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}

public class OrderItemResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}