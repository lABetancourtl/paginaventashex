using PaginaVentasNet.Api.Modules.Payments.Domain;

namespace PaginaVentasNet.Api.Modules.Payments.Application.Ports;

public interface IPaymentTransactionRepository
{
    Task AddAsync(PaymentTransaction transaction);
    Task SaveChangesAsync();
}