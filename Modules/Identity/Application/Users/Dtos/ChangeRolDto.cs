using System.ComponentModel.DataAnnotations;
using PaginaVentasNet.Api.Modules.Identity.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Users.Dtos;

/// <summary>
/// DTO para cambiar el rol de un usuario.
/// </summary>
public class ChangeRolDto
{
    [Required(ErrorMessage = "El rol es obligatorio.")]
    public Rol Rol { get; set; }
}