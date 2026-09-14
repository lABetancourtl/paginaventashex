namespace PaginaVentasNet.Api.Modules.Catalog.Domain;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public int Stock { get; private set; }
    public bool IsActive { get; private set; }
    public int CategoryId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Product() { }

    public static Product Create(
        string name,
        string description,
        decimal price,
        string sku,
        int stock,
        int categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del producto es obligatorio.");

        if (price < 0)
            throw new ArgumentException("El precio no puede ser negativo.");

        if (stock < 0)
            throw new ArgumentException("El stock no puede ser negativo.");

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("El SKU del producto es obligatorio.");

        return new Product
        {
            Name = name.Trim(),
            Description = description.Trim(),
            Price = price,
            Sku = sku.Trim().ToUpper(),
            Stock = stock,
            IsActive = false,
            CategoryId = categoryId,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}