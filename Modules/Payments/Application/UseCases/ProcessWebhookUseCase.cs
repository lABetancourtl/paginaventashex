using System.Text.Json;
using PaginaVentasNet.Api.Modules.Orders.Application.Ports;
using PaginaVentasNet.Api.Modules.Payments.Application.Dtos;
using PaginaVentasNet.Api.Modules.Payments.Application.Ports;
using PaginaVentasNet.Api.Modules.Payments.Domain;

namespace PaginaVentasNet.Api.Modules.Payments.Application.UseCases;

/// <summary>
/// Caso de uso para procesar el webhook de Wompi.
/// Valida la firma, guarda el registro de la transacción y actualiza el pedido si el pago fue exitoso.
/// </summary>
public class ProcessWebhookUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IPaymentTransactionRepository _transactionRepository;

    public ProcessWebhookUseCase(
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        IPaymentTransactionRepository transactionRepository)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _transactionRepository = transactionRepository;
    }

    public async Task ExecuteAsync(WompiWebhookDto dto, string rawPayload)
    {
        var webhookSecret = Environment.GetEnvironmentVariable("WOMPI_EVENTS_SECRET")!;
        var transaction = dto.Data.Transaction;

        var isValid = _paymentService.ValidateWebhookSignature(
            dto.Signature.Checksum,
            dto.Timestamp,
            webhookSecret,
            transaction.Id,
            transaction.Status,
            transaction.AmountInCents);

        if (!isValid)
            throw new UnauthorizedAccessException("Firma del webhook inválida.");

        if (dto.Event != "transaction.updated")
            return;

        int? orderId = int.TryParse(transaction.Reference, out var parsedId) ? parsedId : null;

        var paymentTransaction = PaymentTransaction.Create(
            orderId: orderId,
            wompiTransactionId: transaction.Id,
            reference: transaction.Reference,
            status: transaction.Status,
            amountInCents: transaction.AmountInCents,
            currency: transaction.Currency,
            paymentMethodType: transaction.PaymentMethodType,
            cardBrand: transaction.PaymentMethod?.Extra?.Brand,
            cardLastFour: transaction.PaymentMethod?.Extra?.LastFour,
            cardHolder: transaction.PaymentMethod?.Extra?.CardHolder,
            customerEmail: transaction.CustomerEmail,
            customerPhone: transaction.CustomerData?.PhoneNumber,
            customerName: transaction.CustomerData?.FullName,
            wompiEvent: dto.Event,
            rawPayload: rawPayload);

        await _transactionRepository.AddAsync(paymentTransaction);
        await _transactionRepository.SaveChangesAsync();

        if (transaction.Status != "APPROVED" || orderId is null)
            return;

        var order = await _orderRepository.GetEntityByIdAsync(orderId.Value);

        if (order is null || order.Status != Orders.Domain.Enums.OrderStatus.PendientePago)
            return;

        order.ConfirmPayment();
        await _orderRepository.SaveChangesAsync();
    }
}