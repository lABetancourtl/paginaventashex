using PaginaVentasNet.Api.Modules.Identity.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Identity.Domain;

public class Usuario
{
    public int Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Rol { get; private set; } = string.Empty;
    public DateTime CreadoEn { get; private set; }

    // Datos del perfil
    public string? Nombre { get; private set; }
    public string? Apellido { get; private set; }
    public string? Documento { get; private set; }
    public Genero? Genero { get; private set; }
    public DateOnly? FechaNacimiento { get; private set; }
    public string? Telefono { get; private set; }
    public ICollection<Direccion> Direcciones { get; private set; } = new List<Direccion>();

    private Usuario() { }

    public static Usuario Create(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.");

        return new Usuario
        {
            Email = email.Trim().ToLower(),
            PasswordHash = passwordHash,
            Rol = "Cliente",
            CreadoEn = DateTime.UtcNow
        };
    }

    public void UpdateProfile(
        string? nombre,
        string? apellido,
        string? documento,
        Genero? genero,
        DateOnly? fechaNacimiento,
        string? telefono)
    {
        Nombre = nombre?.Trim();
        Apellido = apellido?.Trim();
        Documento = documento?.Trim();
        Genero = genero;
        FechaNacimiento = fechaNacimiento;
        Telefono = telefono?.Trim();
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("El hash de la contraseña es obligatorio.");

        PasswordHash = newPasswordHash;
    }
}