using Microsoft.AspNetCore.Http;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;

public interface ICloudinaryService
{
    /// <summary>Sube un archivo a la carpeta indicada. Por defecto, la de las fotos de producto.</summary>
    Task<(string Url, string PublicId)> UploadAsync(
        IFormFile file, MediaType mediaType, string folder = "paginaventas/products");

    /// <summary>
    /// Borra un archivo. Cloudinary guarda imágenes y videos como tipos de recurso distintos,
    /// así que hay que indicar el tipo del archivo que se borra.
    /// </summary>
    Task DeleteAsync(string publicId, MediaType mediaType = MediaType.Image);
}