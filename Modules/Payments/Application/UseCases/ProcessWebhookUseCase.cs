using PaginaVentasNet.Api.Modules.Orders.Application.Ports;
using PaginaVentasNet.Api.Modules.Payments.Application.Dtos;
using PaginaVentasNet.Api.Modules.Payments.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Payments.Application.UseCases;

/// <summary>
/// Caso de uso para procesar el webhook de Wompi.
/// Valida la firma y actualiza el estado del pedido si el pago fue exitoso.
/// </summary>
public class ProcessWebhookUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;

    public ProcessWebhookUseCase(
        IOrderRepository orderRepository,
        IPaymentService paymentService)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
    }

    public async Task ExecuteAsync(WompiWebhookDto dto)
    {
        var webhookSecret = Environment.GetEnvironmentVariable("WOMPI_EVENTS_SECRET")!;

        var isValid = _paymentService.ValidateWebhookSignature(
            dto.Signature,
            dto.Timestamp,
            webhookSecret);

        if (!isValid)
            throw new UnauthorizedAccessException("Firma del webhook inválida.");

        if (dto.Event != "transaction.updated")
            return;

        var transaction = dto.Data.Transaction;

        if (transaction.Status != "APPROVED")
            return;

        if (!int.TryParse(transaction.Reference, out var orderId))
            return;

        var order = await _orderRepository.GetEntityByIdAsync(orderId);

        if (order is null)
            return;

        order.ConfirmPayment();
        await _orderRepository.SaveChangesAsync();
    }
}