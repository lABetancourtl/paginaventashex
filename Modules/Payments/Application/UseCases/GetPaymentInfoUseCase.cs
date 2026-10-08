using PaginaVentasNet.Api.Modules.Orders.Application.Ports;
using PaginaVentasNet.Api.Modules.Payments.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Payments.Application.UseCases;

public class PaymentInfoDto
{
    public string PublicKey { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public long AmountInCents { get; set; }
    public string Currency { get; set; } = "COP";
    public string IntegritySignature { get; set; } = string.Empty;
}

/// <summary>
/// Caso de uso para obtener los datos necesarios para iniciar el pago en Wompi.
/// </summary>
public class GetPaymentInfoUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;

    public GetPaymentInfoUseCase(
        IOrderRepository orderRepository,
        IPaymentService paymentService)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
    }

    public async Task<PaymentInfoDto> ExecuteAsync(int orderId, int usuarioId)
    {
        var order = await _orderRepository.GetEntityByIdAsync(orderId);

        if (order is null || order.UsuarioId != usuarioId)
            throw new InvalidOperationException(
                $"No existe un pedido con Id '{orderId}'.");

        if (order.Status != Orders.Domain.Enums.OrderStatus.PendientePago)
            throw new InvalidOperationException(
                "Este pedido ya fue pagado o cancelado.");

        var reference = order.Id.ToString();
        var amountInCents = (long)(order.Total * 100);
        var signature = _paymentService.GenerateIntegritySignature(
            reference, amountInCents, "COP");

        return new PaymentInfoDto
        {
            PublicKey = Environment.GetEnvironmentVariable("WOMPI_PUBLIC_KEY")!,
            Reference = reference,
            AmountInCents = amountInCents,
            Currency = "COP",
            IntegritySignature = signature
        };
    }
}