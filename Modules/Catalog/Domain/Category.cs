namespace PaginaVentasNet.Api.Modules.Catalog.Domain;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public int? ParentCategoryId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Category() { }

    public static Category Create(string name, string slug, int? parentCategoryId = null)
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
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}