using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;


public class CreateCategoryDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El slug es obligatorio.")]
    [StringLength(200, ErrorMessage = "El slug no puede superar 200 caracteres.")]
    public string Slug { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }
}