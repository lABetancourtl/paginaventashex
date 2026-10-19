using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Ports;

namespace PaginaVentasNet.Api.Modules.Geography.Infrastructure;

/// <summary>
/// Implementación del validador geográfico para uso del módulo Identity.
/// </summary>
public class GeographyValidator : IGeographyValidator
{
    private readonly AppDbContext _context;

    public GeographyValidator(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> MunicipioPerteneceAlDepartamentoAsync(int municipioCodigo, int departamentoCodigo)
    {
        return await _context.Municipios
            .AnyAsync(m => m.Codigo == municipioCodigo && m.DepartamentoCodigo == departamentoCodigo);
    }
}