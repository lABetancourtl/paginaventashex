namespace PaginaVentasNet.Api.Modules.Orders.Domain.Enums;

/// <summary>
/// Estado de la devolución del dinero de un pedido cancelado.
/// </summary>
public enum RefundStatus
{
    /// <summary>No hay nada que devolver (el pedido se canceló sin haberse pagado).</summary>
    NotRequired,

    /// <summary>El pedido se canceló estando pagado y falta devolver el dinero.</summary>
    Pending,

    /// <summary>El admin ya devolvió el dinero.</summary>
    Completed
}