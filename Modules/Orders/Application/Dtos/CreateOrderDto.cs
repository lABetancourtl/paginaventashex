using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Orders.Application.Dtos;

/// <summary>
/// DTO para crear un pedido desde el carrito.
/// </summary>
public class CreateOrderDto
{
    [Required(ErrorMessage = "La dirección de entrega es obligatoria.")]
    public int DireccionId { get; set; }
}