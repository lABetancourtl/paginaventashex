namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;

public class PagedResultDto<T>
{
    private const int PageSize = 10;

    public List<T> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}