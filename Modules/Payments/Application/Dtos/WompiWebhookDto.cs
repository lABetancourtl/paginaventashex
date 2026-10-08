namespace PaginaVentasNet.Api.Modules.Payments.Application.Dtos;

/// <summary>
/// DTO que representa el evento que Wompi envía al webhook.
/// </summary>
public class WompiWebhookDto
{
    public string Event { get; set; } = string.Empty;
    public WompiWebhookData Data { get; set; } = new();
    public string Signature { get; set; } = string.Empty;
    public long Timestamp { get; set; }
}

public class WompiWebhookData
{
    public WompiTransaction Transaction { get; set; } = new();
}

public class WompiTransaction
{
    public string Id { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public long AmountInCents { get; set; }
    public string Currency { get; set; } = string.Empty;
}