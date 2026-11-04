namespace PaginaVentasNet.Api.Modules.Catalog.Domain;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
    public int CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public ICollection<ProductMedia> Media { get; private set; } = new List<ProductMedia>();
    public ICollection<ProductVariant> Variants { get; private set; } = new List<ProductVariant>();
    public int? DiscountPercent { get; private set; }

    /// <summary>Precio que realmente paga el cliente. Se calcula, no se guarda.</summary>
    public decimal FinalPrice => PriceCalculator.Final(Price, DiscountPercent);

    private Product() { }

    public static Product Create(
        string name,
        string description,
        decimal price,
        int categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del producto es obligatorio.");

        if (price <= 0)
            throw new ArgumentException("El precio debe ser mayor a cero.");

        return new Product
        {
            Name = name.Trim(),
            Description = description.Trim(),
            Price = price,
            IsActive = false,
            CategoryId = categoryId,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void Update(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del producto es obligatorio.");

        if (price <= 0)
            throw new ArgumentException("El precio debe ser mayor a cero.");

        Name = name.Trim();
        Description = description.Trim();
        Price = price;
    }

    /// <summary>
    /// Agrega la variante inicial del producto, sin talla ni color, con el SKU y el stock indicados.
    /// Se llama al crear el producto: el carrito, el pedido y el stock trabajan sobre variantes.
    /// El SKU y el stock se validan en la variante.
    /// </summary>
    public void AddDefaultVariant(string sku, int stock)
    {
        Variants.Add(ProductVariant.Create(0, null, null, sku, stock));
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("El producto ya está activo.");
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("El producto ya está inactivo.");
        IsActive = false;
    }

    public void ApplyDiscount(int percent)
    {
        if (percent < 1 || percent > 99)
            throw new ArgumentException("El descuento debe estar entre 1 y 99.");

        DiscountPercent = percent;
    }

    public void RemoveDiscount()
    {
        if (DiscountPercent is null)
            throw new InvalidOperationException("El producto no tiene un descuento aplicado.");

        DiscountPercent = null;
    }
}