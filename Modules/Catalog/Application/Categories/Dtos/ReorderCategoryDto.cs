using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;

/// <summary>
/// DTO para mover una categoría a un nuevo orden entre sus hermanas.
/// El Id de la categoría viaja en la URL.
/// </summary>
public class ReorderCategoryDto
{
    [Range(1, int.MaxValue, ErrorMessage = "El orden debe ser mayor a cero.")]
    public int NewDisplayOrder { get; set; }
}