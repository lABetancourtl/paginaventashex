using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Identity.Application.Otp.Ports;
using PaginaVentasNet.Api.Modules.Identity.Domain;

namespace PaginaVentasNet.Api.Modules.Identity.Infrastructure.Otp;

public class OtpRepository : IOtpRepository
{
    private readonly AppDbContext _context;

    public OtpRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OtpCode otpCode)
    {
        await _context.OtpCodes.AddAsync(otpCode);
    }

    public async Task<OtpCode?> GetValidByEmailAsync(string email, string code)
    {
        return await _context.OtpCodes
            .Where(o => o.Email == email.ToLower()
                     && o.Code == code
                     && !o.IsUsed
                     && o.ExpiresAtUtc > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAtUtc)
            .FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}