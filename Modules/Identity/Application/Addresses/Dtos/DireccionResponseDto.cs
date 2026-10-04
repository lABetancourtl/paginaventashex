namespace PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Dtos;

/// <summary>
/// DTO de respuesta para representar una dirección del usuario.
/// </summary>
public class DireccionResponseDto
{
    public int Id { get; set; }
    public int DepartamentoCodigo { get; set; }
    public string DepartamentoNombre { get; set; } = string.Empty;
    public int MunicipioCodigo { get; set; }
    public string MunicipioNombre { get; set; } = string.Empty;
    public string DireccionTexto { get; set; } = string.Empty;
    public string? InformacionAdicional { get; set; }
    public string? Barrio { get; set; }
    public string NombreQuienRecibe { get; set; } = string.Empty;
    public bool EsPrincipal { get; set; }
}