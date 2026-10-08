namespace PaginaVentasNet.Api.Modules.Payments.Domain;

/// <summary>
/// Registra cada transacción de pago recibida desde Wompi.
/// Se guarda independientemente de si el pago fue exitoso o fallido.
/// </summary>
public class PaymentTransaction
{
    public int Id { get; private set; }
    public int? OrderId { get; private set; }
    public string WompiTransactionId { get; private set; } = string.Empty;
    public string Reference { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public long AmountInCents { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string PaymentMethodType { get; private set; } = string.Empty;
    public string? CardBrand { get; private set; }
    public string? CardLastFour { get; private set; }
    public string? CardHolder { get; private set; }
    public string CustomerEmail { get; private set; } = string.Empty;
    public string? CustomerPhone { get; private set; }
    public string? CustomerName { get; private set; }
    public string WompiEvent { get; private set; } = string.Empty;
    public string RawPayload { get; private set; } = string.Empty;
    public DateTime ReceivedAtUtc { get; private set; }

    private PaymentTransaction() { }

    public static PaymentTransaction Create(
        int? orderId,
        string wompiTransactionId,
        string reference,
        string status,
        long amountInCents,
        string currency,
        string paymentMethodType,
        string? cardBrand,
        string? cardLastFour,
        string? cardHolder,
        string customerEmail,
        string? customerPhone,
        string? customerName,
        string wompiEvent,
        string rawPayload)
    {
        return new PaymentTransaction
        {
            OrderId = orderId,
            WompiTransactionId = wompiTransactionId,
            Reference = reference,
            Status = status,
            AmountInCents = amountInCents,
            Currency = currency,
            PaymentMethodType = paymentMethodType,
            CardBrand = cardBrand,
            CardLastFour = cardLastFour,
            CardHolder = cardHolder,
            CustomerEmail = customerEmail,
            CustomerPhone = customerPhone,
            CustomerName = customerName,
            WompiEvent = wompiEvent,
            RawPayload = rawPayload,
            ReceivedAtUtc = DateTime.UtcNow
        };
    }
}