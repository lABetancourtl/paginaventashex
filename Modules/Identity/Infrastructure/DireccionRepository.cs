using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Ports;
using PaginaVentasNet.Api.Modules.Identity.Domain;

namespace PaginaVentasNet.Api.Modules.Identity.Infrastructure;

public class DireccionRepository : IDireccionRepository
{
    private readonly AppDbContext _context;

    public DireccionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Direccion direccion)
    {
        await _context.Direcciones.AddAsync(direccion);
    }

    public async Task<List<DireccionResponseDto>> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _context.Direcciones
            .Where(d => d.UsuarioId == usuarioId)
            .Include(d => d.Departamento)
            .Include(d => d.Municipio)
            .OrderByDescending(d => d.EsPrincipal)
            .ThenBy(d => d.CreadoEn)
            .Select(d => new DireccionResponseDto
            {
                Id = d.Id,
                DepartamentoCodigo = d.DepartamentoCodigo,
                DepartamentoNombre = d.Departamento!.Nombre,
                MunicipioCodigo = d.MunicipioCodigo,
                MunicipioNombre = d.Municipio!.Nombre,
                DireccionTexto = d.DireccionTexto,
                InformacionAdicional = d.InformacionAdicional,
                Barrio = d.Barrio,
                NombreQuienRecibe = d.NombreQuienRecibe,
                EsPrincipal = d.EsPrincipal
            })
            .ToListAsync();
    }

    public async Task<Direccion?> GetEntityByIdAsync(int id)
    {
        return await _context.Direcciones.FindAsync(id);
    }

    public async Task<Direccion?> GetPrincipalByUsuarioIdAsync(int usuarioId)
    {
        return await _context.Direcciones
            .Where(d => d.UsuarioId == usuarioId)
            .OrderBy(d => d.CreadoEn)
            .FirstOrDefaultAsync();
    }

    public Task RemoveAsync(Direccion direccion)
    {
        _context.Direcciones.Remove(direccion);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}