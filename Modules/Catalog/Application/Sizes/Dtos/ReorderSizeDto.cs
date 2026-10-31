using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Dtos;

public class ReorderSizeDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El orden debe ser mayor a cero.")]
    public int NewDisplayOrder { get; set; }
}