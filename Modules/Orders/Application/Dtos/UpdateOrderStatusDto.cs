using System.ComponentModel.DataAnnotations;
using PaginaVentasNet.Api.Modules.Orders.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Orders.Application.Dtos;

public class UpdateOrderStatusDto
{
    [Required(ErrorMessage = "El estado es obligatorio.")]
    public OrderStatus Status { get; set; }
}