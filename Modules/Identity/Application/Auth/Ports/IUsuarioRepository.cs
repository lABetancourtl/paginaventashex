using PaginaVentasNet.Api.Modules.Identity.Domain;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Auth.Ports;

public interface IUsuarioRepository
{
    Task AddAsync(Usuario usuario);
    Task<bool> ExistsByEmailAsync(string email);
    Task<Usuario?> GetByEmailAsync(string email);
    Task SaveChangesAsync();
}