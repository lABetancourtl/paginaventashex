using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Cart.Application.Dtos;

public class UpdateCartItemDto
{
    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, 100, ErrorMessage = "La cantidad debe ser entre 1 y 100.")]
    public int Quantity { get; set; }
}