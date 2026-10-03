namespace PaginaVentasNet.Api.Modules.Geography.Domain;

/// <summary>
/// Representa un municipio de Colombia según DIVIPOLA del DANE.
/// </summary>
public class Municipio
{
    public int Codigo { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public int DepartamentoCodigo { get; private set; }
    public Departamento? Departamento { get; private set; }

    private Municipio() { }

    public static Municipio Create(int codigo, string nombre, int departamentoCodigo)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del municipio es obligatorio.");

        return new Municipio
        {
            Codigo = codigo,
            Nombre = nombre.Trim(),
            DepartamentoCodigo = departamentoCodigo
        };
    }
}