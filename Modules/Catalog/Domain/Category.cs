namespace PaginaVentasNet.Api.Modules.Catalog.Domain;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public int? ParentCategoryId { get; private set; }
    public bool IsActive { get; private set; }

    /// <summary>Orden de visualización entre categorías hermanas (mismo padre).</summary>
    public int DisplayOrder { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public Category? ParentCategory { get; private set; }
    public ICollection<Category> ChildCategories { get; private set; } = new List<Category>();

    private Category() { }

    public static Category Create(string name, string slug, int? parentCategoryId = null, int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la categoría es obligatorio.");

        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("El slug de la categoría es obligatorio.");

        return new Category
        {
            Name = name.Trim(),
            Slug = slug.Trim().ToLower(),
            ParentCategoryId = parentCategoryId,
            DisplayOrder = displayOrder,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void Update(string name, string slug)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la categoría es obligatorio.");

        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("El slug de la categoría es obligatorio.");

        Name = name.Trim();
        Slug = slug.Trim().ToLower();
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
            throw new InvalidOperationException("La categoría ya está activa.");
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("La categoría ya está desactivada.");
        IsActive = false;
    }
}