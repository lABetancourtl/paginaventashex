using PaginaVentasNet.Api.Modules.Payments.Application.Dtos;
using PaginaVentasNet.Api.Modules.Payments.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Payments.Application.UseCases;

/// <summary>
/// Caso de uso para obtener el historial de transacciones de pago.
/// Solo accesible por administradores.
/// </summary>
public class GetPaymentTransactionsUseCase
{
    private readonly IPaymentTransactionRepository _repository;

    public GetPaymentTransactionsUseCase(IPaymentTransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PaymentTransactionResponseDto>> ExecuteAsync()
    {
        return await _repository.GetAllAsync();
    }
}