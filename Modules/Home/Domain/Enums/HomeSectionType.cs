namespace PaginaVentasNet.Api.Modules.Home.Domain.Enums;

/// <summary>
/// Tipo de bloque de la página de inicio.
/// </summary>
public enum HomeSectionType
{
    /// <summary>De 1 a 4 medios (imagen o video). Cada uno puede enlazar a una categoría.</summary>
    Banner,

    /// <summary>Un título opcional y hasta 4 productos elegidos por el admin.</summary>
    Products
}