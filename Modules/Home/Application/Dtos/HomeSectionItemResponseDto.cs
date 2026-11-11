using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Home.Application.Dtos;

public class HomeSectionItemResponseDto
{
    public int Id { get; set; }
    public int SectionId { get; set; }
    public string Url { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategorySlug { get; set; }
    public int DisplayOrder { get; set; }
}