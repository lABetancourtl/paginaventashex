using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Geography.Application.Municipios.Dtos;
using PaginaVentasNet.Api.Modules.Geography.Application.Municipios.Ports;

namespace PaginaVentasNet.Api.Modules.Geography.Infrastructure;

public class MunicipioRepository : IMunicipioRepository
{
    private readonly AppDbContext _context;

    public MunicipioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MunicipioDto>> GetByDepartamentoAsync(int departamentoCodigo)
    {
        return await _context.Municipios
            .Where(m => m.DepartamentoCodigo == departamentoCodigo)
            .OrderBy(m => m.Nombre)
            .Select(m => new MunicipioDto
            {
                Codigo = m.Codigo,
                Nombre = m.Nombre,
                DepartamentoCodigo = m.DepartamentoCodigo
            })
            .ToListAsync();
    }
}