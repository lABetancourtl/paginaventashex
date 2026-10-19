namespace PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Ports;

/// <summary>
/// Puerto de salida para validar datos geográficos desde el módulo Identity.
/// La implementación vive en el módulo Geography.
/// </summary>
public interface IGeographyValidator
{
    Task<bool> MunicipioPerteneceAlDepartamentoAsync(int municipioCodigo, int departamentoCodigo);
}