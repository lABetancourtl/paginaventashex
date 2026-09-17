using PaginaVentasNet.Api.Modules.Identity.Domain;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Auth.Ports;

public interface IJwtService
{
    string GenerarToken(Usuario usuario);
}