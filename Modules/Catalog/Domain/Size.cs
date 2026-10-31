using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Domain;

/// <summary>
/// Talla que el admin puede asignar a una variante de producto.
/// El orden (DisplayOrder) se numera dentro de cada grupo.
/// </summary>
public class Size
{
    public int Id { get; private set; }
    public SizeGroup SizeGroup { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }

    private Size() { }

    public static Size Create(SizeGroup sizeGroup, string name, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la talla es obligatorio.");

        if (displayOrder < 1)
            throw new ArgumentException("El orden debe ser mayor a cero.");

        return new Size
        {
            SizeGroup = sizeGroup,
            Name = name.Trim(),
            DisplayOrder = displayOrder,
            IsActive = true
        };
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la talla es obligatorio.");

        Name = name.Trim();
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
            throw new InvalidOperationException("La talla ya está activa.");
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("La talla ya está desactivada.");
        IsActive = false;
    }
}