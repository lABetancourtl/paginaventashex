namespace PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Dtos;

public class ColorResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? HexCode { get; set; }
    public bool IsActive { get; set; }
}