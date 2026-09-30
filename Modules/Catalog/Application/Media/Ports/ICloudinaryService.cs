using Microsoft.AspNetCore.Http;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;

/// <summary>
/// Puerto de salida para el servicio de almacenamiento de archivos en Cloudinary.
/// </summary>
public interface ICloudinaryService
{
    Task<(string Url, string PublicId)> UploadAsync(IFormFile file, MediaType mediaType);
    Task DeleteAsync(string publicId);
}