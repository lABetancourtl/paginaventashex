using PaginaVentasNet.Api.Modules.Orders.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Orders.Domain;

/// <summary>
/// Representa un pedido realizado por un usuario.
/// Los datos de dirección y productos se copian al momento de crear el pedido.
/// </summary>
public class Order
{
    public int Id { get; private set; }
    public int UsuarioId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal Total { get; private set; }

    // Datos de entrega copiados de la dirección seleccionada
    public string DepartamentoNombre { get; private set; } = string.Empty;
    public string MunicipioNombre { get; private set; } = string.Empty;
    public string DireccionTexto { get; private set; } = string.Empty;
    public string? InformacionAdicional { get; private set; }
    public string? Barrio { get; private set; }
    public string NombreQuienRecibe { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }
    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    private Order() { }

    public static Order Create(
        int usuarioId,
        string departamentoNombre,
        string municipioNombre,
        string direccionTexto,
        string nombreQuienRecibe,
        string? informacionAdicional = null,
        string? barrio = null)
    {
        if (string.IsNullOrWhiteSpace(direccionTexto))
            throw new ArgumentException("La dirección es obligatoria.");

        if (string.IsNullOrWhiteSpace(nombreQuienRecibe))
            throw new ArgumentException("El nombre de quien recibe es obligatorio.");

        return new Order
        {
            UsuarioId = usuarioId,
            Status = OrderStatus.PendientePago,
            Total = 0,
            DepartamentoNombre = departamentoNombre,
            MunicipioNombre = municipioNombre,
            DireccionTexto = direccionTexto,
            InformacionAdicional = informacionAdicional,
            Barrio = barrio,
            NombreQuienRecibe = nombreQuienRecibe,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void SetTotal(decimal total)
    {
        if (total <= 0)
            throw new ArgumentException("El total debe ser mayor a cero.");
        Total = total;
    }

    public void UpdateStatus(OrderStatus status)
    {
        if (Status == OrderStatus.Cancelado)
            throw new InvalidOperationException(
                "No se puede cambiar el estado de un pedido cancelado.");

        if (Status == OrderStatus.PendientePago)
            throw new InvalidOperationException(
                "No se puede cambiar el estado de un pedido con pago pendiente.");

        Status = status;
    }

    public void ConfirmPayment()
    {
        if (Status != OrderStatus.PendientePago)
            throw new InvalidOperationException(
                "El pedido no está en estado de pago pendiente.");

        Status = OrderStatus.Confirmado;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Enviado)
            throw new InvalidOperationException(
                "No se puede cancelar un pedido que ya fue enviado.");

        if (Status == OrderStatus.Cancelado)
            throw new InvalidOperationException(
                "El pedido ya está cancelado.");

        Status = OrderStatus.Cancelado;
    }
}