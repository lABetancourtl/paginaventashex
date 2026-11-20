using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Home.Application.Dtos;

/// <summary>
/// La lista completa de productos de un estante, en el orden en que se muestran.
/// Reemplaza la lista anterior. Una lista vacía deja el estante sin productos.
/// </summary>
public class SetHomeSectionProductsDto
{
    [Required(ErrorMessage = "La lista de productos es obligatoria.")]
    public List<int> ProductIds { get; set; } = null!;
}