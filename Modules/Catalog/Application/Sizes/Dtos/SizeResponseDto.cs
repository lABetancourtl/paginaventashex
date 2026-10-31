using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Dtos;

public class SizeResponseDto
{
    public int Id { get; set; }
    public SizeGroup SizeGroup { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}