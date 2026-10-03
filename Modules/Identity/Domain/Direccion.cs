using PaginaVentasNet.Api.Modules.Geography.Domain;

namespace PaginaVentasNet.Api.Modules.Identity.Domain;

/// <summary>
/// Representa una dirección de entrega del usuario.
/// </summary>
public class Direccion
{
    public int Id { get; private set; }
    public int UsuarioId { get; private set; }
    public int DepartamentoCodigo { get; private set; }
    public int MunicipioCodigo { get; private set; }
    public Departamento? Departamento { get; private set; }
    public Municipio? Municipio { get; private set; }
    public string DireccionTexto { get; private set; } = string.Empty;
    public string? InformacionAdicional { get; private set; }
    public string? Barrio { get; private set; }
    public string NombreQuienRecibe { get; private set; } = string.Empty;
    public bool EsPrincipal { get; private set; }
    public DateTime CreadoEn { get; private set; }

    private Direccion() { }

    public static Direccion Create(
        int usuarioId,
        int departamentoCodigo,
        int municipioCodigo,
        string direccionTexto,
        string nombreQuienRecibe,
        string? informacionAdicional = null,
        string? barrio = null,
        bool esPrincipal = false)
    {
        if (string.IsNullOrWhiteSpace(direccionTexto))
            throw new ArgumentException("La dirección es obligatoria.");

        if (string.IsNullOrWhiteSpace(nombreQuienRecibe))
            throw new ArgumentException("El nombre de quien recibe es obligatorio.");

        return new Direccion
        {
            UsuarioId = usuarioId,
            DepartamentoCodigo = departamentoCodigo,
            MunicipioCodigo = municipioCodigo,
            DireccionTexto = direccionTexto.Trim(),
            NombreQuienRecibe = nombreQuienRecibe.Trim(),
            InformacionAdicional = informacionAdicional?.Trim(),
            Barrio = barrio?.Trim(),
            EsPrincipal = esPrincipal,
            CreadoEn = DateTime.UtcNow
        };
    }

    public void SetAsPrincipal()
    {
        EsPrincipal = true;
    }

    public void RemoveAsPrincipal()
    {
        EsPrincipal = false;
    }

    public void Update(
        int departamentoCodigo,
        int municipioCodigo,
        string direccionTexto,
        string nombreQuienRecibe,
        string? informacionAdicional = null,
        string? barrio = null)
    {
        if (string.IsNullOrWhiteSpace(direccionTexto))
            throw new ArgumentException("La dirección es obligatoria.");

        if (string.IsNullOrWhiteSpace(nombreQuienRecibe))
            throw new ArgumentException("El nombre de quien recibe es obligatorio.");

        DepartamentoCodigo = departamentoCodigo;
        MunicipioCodigo = municipioCodigo;
        DireccionTexto = direccionTexto.Trim();
        NombreQuienRecibe = nombreQuienRecibe.Trim();
        InformacionAdicional = informacionAdicional?.Trim();
        Barrio = barrio?.Trim();
    }
}