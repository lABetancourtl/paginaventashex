namespace PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

/// <summary>
/// Define el rol de una imagen dentro del catálogo.
/// </summary>
public enum DisplayRole
{
    /// <summary>Foto de galería normal — visible solo en el detalle del producto.</summary>
    None,

    /// <summary>Foto principal del catálogo — se muestra por defecto en la tarjeta.</summary>
    CatalogMain,

    /// <summary>Foto del hover — se muestra al pasar el mouse sobre la tarjeta.</summary>
    CatalogHover
}