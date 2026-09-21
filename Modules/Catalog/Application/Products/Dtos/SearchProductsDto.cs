namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;

public class SearchProductsDto
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
}