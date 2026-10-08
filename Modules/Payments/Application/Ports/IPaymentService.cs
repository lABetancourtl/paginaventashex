namespace PaginaVentasNet.Api.Modules.Payments.Application.Ports;

public interface IPaymentService
{
   bool ValidateWebhookSignature(
    string checksum,
    long timestamp,
    string webhookSecret,
    string transactionId,
    string transactionStatus,
    long amountInCents);
    
    string GenerateIntegritySignature(string reference, long amountInCents, string currency);
}