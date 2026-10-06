using PaginaVentasNet.Api.Modules.Identity.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Identity.Domain;

/// <summary>
/// Representa un usuario en el sistema.
/// </summary>
public class Usuario
{
    public int Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public Rol Rol { get; private set; }
    public DateTime CreadoEn { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Datos del perfil
    public string? Nombre { get; private set; }
    public string? Apellido { get; private set; }
    public string? Documento { get; private set; }
    public Genero? Genero { get; private set; }
    public DateOnly? FechaNacimiento { get; private set; }
    public string? Telefono { get; private set; }
    public ICollection<Direccion> Direcciones { get; private set; } = new List<Direccion>();

    private Usuario() { }


    /// <summary>
    /// Crea una nueva instancia de Usuario con el email y el hash de la contraseña proporcionados.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="passwordHash"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Usuario Create(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.");

        return new Usuario
        {
            Email = email.Trim().ToLower(),
            PasswordHash = passwordHash,
            Rol = Rol.Cliente,
            CreadoEn = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Desactiva la cuenta del usuario.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("La cuenta ya está desactivada.");
        IsActive = false;
    }

    /// <summary>
    /// Actualiza el perfil del usuario con la información proporcionada. Los parámetros pueden ser nulos, en cuyo caso no se actualizarán.
    /// </summary>
    /// <param name="nombre"></param>
    /// <param name="apellido"></param>
    /// <param name="documento"></param>
    /// <param name="genero"></param>
    /// <param name="fechaNacimiento"></param>
    /// <param name="telefono"></param>
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

    /// <summary>
    /// Actualiza el hash de la contraseña del usuario. El nuevo hash no puede ser nulo o vacío.
    /// </summary>
    /// <param name="newPasswordHash"></param>
    /// <exception cref="ArgumentException"></exception>
    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("El hash de la contraseña es obligatorio.");

        PasswordHash = newPasswordHash;
    }

    /// <summary>
    /// Cambia el rol del usuario al rol proporcionado.
    /// </summary>
    /// <param name="rol"></param>
    public void ChangeRol(Rol rol)
    {
        Rol = rol;
    }
}