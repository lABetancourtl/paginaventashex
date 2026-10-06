using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Domain;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Ports;

/// <summary>
/// Repositorio para manejar las operaciones de persistencia relacionadas con las direcciones de los usuarios.
/// </summary>
public interface IDireccionRepository
{
    Task AddAsync(Direccion direccion);
    Task<List<DireccionResponseDto>> GetByUsuarioIdAsync(int usuarioId);
    Task<Direccion?> GetEntityByIdAsync(int id);
    Task<Direccion?> GetPrincipalByUsuarioIdAsync(int usuarioId);
    Task RemoveAsync(Direccion direccion);
    Task SaveChangesAsync();
    Task<string> GetDepartamentoNombreAsync(int departamentoCodigo);
    Task<string> GetMunicipioNombreAsync(int municipioCodigo);
}