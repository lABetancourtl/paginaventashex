namespace PaginaVentasNet.Api.Modules.Identity.Domain;

public class Usuario
{
    public int Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Rol { get; private set; } = string.Empty;
    public DateTime CreadoEn { get; private set; }

    private Usuario() { }

    public static Usuario Create(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El password es obligatorio.");

        return new Usuario
        {
            Email = email.Trim().ToLower(),
            PasswordHash = passwordHash,
            Rol = "Cliente",
            CreadoEn = DateTime.UtcNow
        };
    }
}