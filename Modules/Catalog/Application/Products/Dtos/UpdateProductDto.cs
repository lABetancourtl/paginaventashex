namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}