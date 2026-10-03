namespace PaginaVentasNet.Api.Modules.Geography.Domain;

/// <summary>
/// Representa un departamento de Colombia según DIVIPOLA del DANE.
/// </summary>
public class Departamento
{
    public int Codigo { get; private set; }
    public string Nombre { get; private set; } = string.Empty;

    private Departamento() { }

    public static Departamento Create(int codigo, string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del departamento es obligatorio.");

        return new Departamento
        {
            Codigo = codigo,
            Nombre = nombre.Trim()
        };
    }

    public ICollection<Municipio> Municipios { get; private set; } = new List<Municipio>();
}