namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;

/// <summary>
/// DTO para búsqueda de productos desde el panel de administración.
/// Permite filtrar por múltiples categorías, nombre y estado.
/// </summary>
public class AdminSearchProductsDto
{
    public string? Search { get; set; }
    public List<int>? CategoryIds { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
}