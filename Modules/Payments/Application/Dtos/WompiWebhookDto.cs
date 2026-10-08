using System.Text.Json.Serialization;

namespace PaginaVentasNet.Api.Modules.Payments.Application.Dtos;

public class WompiWebhookDto
{
    public string Event { get; set; } = string.Empty;
    public WompiWebhookData Data { get; set; } = new();
    public WompiWebhookSignature Signature { get; set; } = new();
    public long Timestamp { get; set; }
}

public class WompiWebhookSignature
{
    public string Checksum { get; set; } = string.Empty;
    public List<string> Properties { get; set; } = new();
}

public class WompiWebhookData
{
    public WompiTransaction Transaction { get; set; } = new();
}

public class WompiTransaction
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("amount_in_cents")]
    public long AmountInCents { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("payment_method_type")]
    public string PaymentMethodType { get; set; } = string.Empty;

    [JsonPropertyName("payment_method")]
    public WompiPaymentMethod? PaymentMethod { get; set; }

    [JsonPropertyName("customer_email")]
    public string CustomerEmail { get; set; } = string.Empty;

    [JsonPropertyName("customer_data")]
    public WompiCustomerData? CustomerData { get; set; }
}

public class WompiPaymentMethod
{
    [JsonPropertyName("extra")]
    public WompiCardExtra? Extra { get; set; }
}

public class WompiCardExtra
{
    [JsonPropertyName("brand")]
    public string? Brand { get; set; }

    [JsonPropertyName("last_four")]
    public string? LastFour { get; set; }

    [JsonPropertyName("card_holder")]
    public string? CardHolder { get; set; }
}

public class WompiCustomerData
{
    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }
}