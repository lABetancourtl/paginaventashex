namespace PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;

/// <summary>
/// DTO que representa una categoría con su jerarquía de hijos.
/// </summary>
public class CategoryTreeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public bool IsActive { get; set; }
    public List<CategoryTreeDto> Children { get; set; } = new();
}