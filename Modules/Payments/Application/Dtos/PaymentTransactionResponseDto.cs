namespace PaginaVentasNet.Api.Modules.Payments.Application.Dtos;

/// <summary>
/// DTO de respuesta para el historial de transacciones de pago.
/// </summary>
public class PaymentTransactionResponseDto
{
    public int Id { get; set; }
    public int? OrderId { get; set; }
    public string WompiTransactionId { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string PaymentMethodType { get; set; } = string.Empty;
    public string? CardBrand { get; set; }
    public string? CardLastFour { get; set; }
    public string? CardHolder { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string? CustomerName { get; set; }
    public DateTime ReceivedAtUtc { get; set; }
}