using PaginaVentasNet.Api.Modules.Identity.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Auth.Dtos;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Rol Rol { get; set; }
    public DateTime Expiracion { get; set; }
}