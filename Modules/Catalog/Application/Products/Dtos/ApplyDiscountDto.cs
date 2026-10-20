using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;

public class ApplyDiscountDto
{
    [Range(1, 99, ErrorMessage = "El descuento debe estar entre 1 y 99.")]
    public int DiscountPercent { get; set; }
}