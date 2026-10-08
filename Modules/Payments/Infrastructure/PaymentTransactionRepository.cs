using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Payments.Application.Dtos;
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

    public async Task<List<PaymentTransactionResponseDto>> GetAllAsync()
    {
        return await _context.PaymentTransactions
            .OrderByDescending(t => t.ReceivedAtUtc)
            .Select(t => new PaymentTransactionResponseDto
            {
                Id = t.Id,
                OrderId = t.OrderId,
                WompiTransactionId = t.WompiTransactionId,
                Reference = t.Reference,
                Status = t.Status,
                Amount = t.AmountInCents / 100,
                Currency = t.Currency,
                PaymentMethodType = t.PaymentMethodType,
                CardBrand = t.CardBrand,
                CardLastFour = t.CardLastFour,
                CardHolder = t.CardHolder,
                CustomerEmail = t.CustomerEmail,
                CustomerPhone = t.CustomerPhone,
                CustomerName = t.CustomerName,
                ReceivedAtUtc = t.ReceivedAtUtc
            })
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}