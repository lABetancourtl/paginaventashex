using System.Security.Cryptography;
using System.Text;
using PaginaVentasNet.Api.Modules.Payments.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Payments.Infrastructure;

/// <summary>
/// Servicio para validar firmas y generar checksums de Wompi.
/// </summary>
public class WompiService : IPaymentService
{
    public bool ValidateWebhookSignature(
        string checksum, 
        long timestamp, 
        string webhookSecret,
        string transactionId,
        string transactionStatus,
        long amountInCents)
    {
        var toSign = $"{transactionId}{transactionStatus}{amountInCents}{timestamp}{webhookSecret}";
        var hash = ComputeSha256(toSign);
        return hash.Equals(checksum, StringComparison.OrdinalIgnoreCase);
    }

    public string GenerateIntegritySignature(string reference, long amountInCents, string currency)
    {
        var integritySecret = Environment.GetEnvironmentVariable("WOMPI_INTEGRITY_SECRET")!;
        var toSign = $"{reference}{amountInCents}{currency}{integritySecret}";
        return ComputeSha256(toSign);
    }

    private static string ComputeSha256(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLower();
    }
}