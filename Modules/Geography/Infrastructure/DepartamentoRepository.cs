using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Geography.Application.Departamentos.Dtos;
using PaginaVentasNet.Api.Modules.Geography.Application.Departamentos.Ports;

namespace PaginaVentasNet.Api.Modules.Geography.Infrastructure;

public class DepartamentoRepository : IDepartamentoRepository
{
    private readonly AppDbContext _context;

    public DepartamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DepartamentoDto>> GetAllAsync()
    {
        return await _context.Departamentos
            .OrderBy(d => d.Nombre)
            .Select(d => new DepartamentoDto
            {
                Codigo = d.Codigo,
                Nombre = d.Nombre
            })
            .ToListAsync();
    }
}