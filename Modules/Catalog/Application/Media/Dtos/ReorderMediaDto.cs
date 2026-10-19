using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;

/// <summary>
/// DTO para intercambiar el orden de una imagen con la que ocupa el orden destino.
/// </summary>
public class ReorderMediaDto
{
    [Required]
    public int MediaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El orden debe ser mayor a cero.")]
    public int NewDisplayOrder { get; set; }
}