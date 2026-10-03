using System.ComponentModel.DataAnnotations;

namespace PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Dtos;

public class CreateDireccionDto
{
    [Required(ErrorMessage = "El código del departamento es obligatorio.")]
    public int DepartamentoCodigo { get; set; }

    [Required(ErrorMessage = "El código del municipio es obligatorio.")]
    public int MunicipioCodigo { get; set; }

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(200, ErrorMessage = "La dirección no puede superar 200 caracteres.")]
    public string DireccionTexto { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "La información adicional no puede superar 100 caracteres.")]
    public string? InformacionAdicional { get; set; }

    [StringLength(100, ErrorMessage = "El barrio no puede superar 100 caracteres.")]
    public string? Barrio { get; set; }

    [Required(ErrorMessage = "El nombre de quien recibe es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    public string NombreQuienRecibe { get; set; } = string.Empty;
}