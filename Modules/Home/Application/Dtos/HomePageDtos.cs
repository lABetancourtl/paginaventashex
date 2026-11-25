using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;
using PaginaVentasNet.Api.Modules.Home.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Home.Application.Dtos;

public class HomePageResponseDto
{
    public List<HomePageSectionDto> Sections { get; set; } = new();
}

public class HomePageSectionDto
{
    public int Id { get; set; }
    public HomeSectionType Type { get; set; }
    public string? Title { get; set; }

    /// <summary>Botón "Ver todo" de un estante. Es null si no tiene o si su categoría está desactivada.</summary>
    public string? ViewAllCategorySlug { get; set; }
    public string? ViewAllCategoryName { get; set; }

    /// <summary>Medios de un banner. Vacío en un estante.</summary>
    public List<HomePageItemDto> Items { get; set; } = new();

    /// <summary>Productos visibles de un estante. Vacío en un banner.</summary>
    public List<HomePageProductDto> Products { get; set; } = new();
}

public class HomePageItemDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }

    /// <summary>
    /// Imagen 4:5 para celular. Solo viene cuando el banner tiene un único medio;
    /// si es null, en celular se usa Url.
    /// </summary>
    public string? MobileUrl { get; set; }

    /// <summary>Categoría a la que lleva el medio. Es null si no enlaza o si su categoría está desactivada.</summary>
    public string? CategorySlug { get; set; }
    public string? CategoryName { get; set; }
}

public class HomePageProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>Subcategoría del producto: la tarjeta la muestra en lugar del nombre, como en el catálogo.</summary>
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int? DiscountPercent { get; set; }

    /// <summary>Precio que paga el cliente. Se calcula con la misma fórmula del dominio.</summary>
    public decimal FinalPrice => PriceCalculator.Final(Price, DiscountPercent);

    public string? CatalogMainUrl { get; set; }
    public string? CatalogHoverUrl { get; set; }
    public bool InStock { get; set; }
}