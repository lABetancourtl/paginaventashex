using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;

public class ReorderMediaDto
{
    [Required]
    public List<MediaOrderItem> Items { get; set; } = new();
}

public class MediaOrderItem
{
    [Required]
    public int MediaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El orden debe ser mayor a cero.")]
    public int DisplayOrder { get; set; }
}