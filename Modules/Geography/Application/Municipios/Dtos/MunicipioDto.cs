namespace PaginaVentasNet.Api.Modules.Geography.Application.Municipios.Dtos;

public class MunicipioDto
{
    public int Codigo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int DepartamentoCodigo { get; set; }
}