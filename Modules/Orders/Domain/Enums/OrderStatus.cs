namespace PaginaVentasNet.Api.Modules.Orders.Domain.Enums;

/// <summary>
/// Define los estados posibles de un pedido.
/// </summary>
public enum OrderStatus
{
    Pendiente,
    Confirmado,
    Enviado,
    Entregado,
    Cancelado
}