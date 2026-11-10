using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Home.Application.Dtos;

public class ReorderHomeSectionDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El orden debe ser mayor a cero.")]
    public int NewDisplayOrder { get; set; }
}