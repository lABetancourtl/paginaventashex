using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;

/// <summary>
/// DTO de respuesta para un archivo multimedia de un producto.
/// </summary>
public class ProductMediaResponseDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}