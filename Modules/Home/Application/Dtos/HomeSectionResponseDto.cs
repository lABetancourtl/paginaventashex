using PaginaVentasNet.Api.Modules.Home.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Home.Application.Dtos;

public class HomeSectionResponseDto
{
    public int Id { get; set; }
    public HomeSectionType Type { get; set; }
    public string? Title { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategorySlug { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    /// <summary>Cantidad de medios (solo tiene sentido en un banner).</summary>
    public int ItemCount { get; set; }

    /// <summary>Cantidad de productos (solo tiene sentido en un estante de productos).</summary>
    public int ProductCount { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}