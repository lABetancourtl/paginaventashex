using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Home.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Home.Domain;

/// <summary>
/// Un bloque de la página de inicio. El admin decide su tipo, su orden y si se muestra.
/// Un banner lleva medios (Items). Un estante de productos lleva productos (Products),
/// un título opcional y un botón "Ver todo" opcional hacia una categoría.
/// </summary>
public class HomeSection
{
    public const int MaxProducts = 4;
    public const int MaxBannerItems = 4;
    public const int MaxTitleLength = 100;

    public int Id { get; private set; }
    public HomeSectionType Type { get; private set; }

    /// <summary>Título del estante. Solo aplica a secciones de productos.</summary>
    public string? Title { get; private set; }

    /// <summary>Categoría del botón "Ver todo". Solo aplica a secciones de productos.</summary>
    public int? CategoryId { get; private set; }
    public Category? Category { get; private set; }

    /// <summary>Orden de la sección en la página, de arriba hacia abajo.</summary>
    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public ICollection<HomeSectionItem> Items { get; private set; } = new List<HomeSectionItem>();
    public ICollection<HomeSectionProduct> Products { get; private set; } = new List<HomeSectionProduct>();

    private HomeSection() { }

    /// <summary>Crea una sección inactiva: el admin la llena y luego la activa.</summary>
    public static HomeSection Create(
        HomeSectionType type,
        string? title,
        int? categoryId,
        int displayOrder)
    {
        if (!Enum.IsDefined(type))
            throw new ArgumentException("El tipo de sección no es válido.");

        if (displayOrder < 1)
            throw new ArgumentException("El orden debe ser mayor a cero.");

        var cleanTitle = NormalizeTitle(title);

        if (type == HomeSectionType.Banner && (cleanTitle is not null || categoryId is not null))
            throw new ArgumentException(
                "Un banner no lleva título ni botón 'Ver todo'. Esos campos son de las secciones de productos.");

        return new HomeSection
        {
            Type = type,
            Title = cleanTitle,
            CategoryId = categoryId,
            DisplayOrder = displayOrder,
            IsActive = false,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    /// <summary>Cambia el título y el botón "Ver todo" de una sección de productos.</summary>
    public void Update(string? title, int? categoryId)
    {
        if (Type != HomeSectionType.Products)
            throw new InvalidOperationException(
                "Solo las secciones de productos tienen título y botón 'Ver todo'.");

        Title = NormalizeTitle(title);
        CategoryId = categoryId;
    }

    public void SetDisplayOrder(int order)
    {
        if (order < 1)
            throw new ArgumentException("El orden debe ser mayor a cero.");

        DisplayOrder = order;
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("La sección ya está activa.");
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("La sección ya está desactivada.");
        IsActive = false;
    }

    private static string? NormalizeTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return null;

        var clean = title.Trim();

        if (clean.Length > MaxTitleLength)
            throw new ArgumentException($"El título no puede superar {MaxTitleLength} caracteres.");

        return clean;
    }
}