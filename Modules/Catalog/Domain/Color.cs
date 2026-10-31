using System.Text.RegularExpressions;

namespace PaginaVentasNet.Api.Modules.Catalog.Domain;

/// <summary>
/// Color que el admin puede asignar a una variante de producto.
/// HexCode es opcional y sirve para pintar la muestra de color en el frontend.
/// </summary>
public class Color
{
    private static readonly Regex HexPattern = new("^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled);

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? HexCode { get; private set; }
    public bool IsActive { get; private set; }

    private Color() { }

    public static Color Create(string name, string? hexCode = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del color es obligatorio.");

        return new Color
        {
            Name = name.Trim(),
            HexCode = NormalizeHex(hexCode),
            IsActive = true
        };
    }

    public void Update(string name, string? hexCode)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del color es obligatorio.");

        Name = name.Trim();
        HexCode = NormalizeHex(hexCode);
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("El color ya está activo.");
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("El color ya está desactivado.");
        IsActive = false;
    }

    private static string? NormalizeHex(string? hexCode)
    {
        if (string.IsNullOrWhiteSpace(hexCode))
            return null;

        var hex = hexCode.Trim();

        if (!HexPattern.IsMatch(hex))
            throw new ArgumentException("El código de color debe tener el formato #RRGGBB, por ejemplo #1E90FF.");

        return hex.ToUpper();
    }
}