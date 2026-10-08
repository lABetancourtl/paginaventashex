namespace PaginaVentasNet.Api.Modules.Orders.Domain.Enums;

/// <summary>
/// Define los estados posibles de un pedido.
/// </summary>
public enum OrderStatus
{
    PendientePago,
    Confirmado,
    Enviado,
    Cancelado
}