namespace PaginaVentasNet.Api.Modules.Identity.Domain;

/// <summary>
/// Representa un código OTP de un solo uso para autenticación sin contraseña.
/// </summary>
public class OtpCode
{
    public int Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private OtpCode() { }

    public static OtpCode Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.");

        return new OtpCode
        {
            Email = email.Trim().ToLower(),
            Code = GenerateCode(),
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }

    public bool IsValid()
    {
        return !IsUsed && DateTime.UtcNow < ExpiresAtUtc;
    }

    private static string GenerateCode()
    {
        return Random.Shared.Next(100000, 999999).ToString();
    }
}