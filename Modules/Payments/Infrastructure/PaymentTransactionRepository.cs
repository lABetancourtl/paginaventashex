using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Payments.Application.Ports;
using PaginaVentasNet.Api.Modules.Payments.Domain;

namespace PaginaVentasNet.Api.Modules.Payments.Infrastructure;

public class PaymentTransactionRepository : IPaymentTransactionRepository
{
    private readonly AppDbContext _context;

    public PaymentTransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PaymentTransaction transaction)
    {
        await _context.PaymentTransactions.AddAsync(transaction);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}