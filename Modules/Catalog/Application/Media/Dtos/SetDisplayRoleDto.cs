using System.ComponentModel.DataAnnotations;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;

public class SetDisplayRoleDto
{
    [Required(ErrorMessage = "El rol es obligatorio.")]
    public DisplayRole Role { get; set; }
}