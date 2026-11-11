using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Media;

/// <summary>
/// Adaptador de salida para el servicio de almacenamiento de archivos en Cloudinary.
/// </summary>
public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService()
    {
        var cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME")!;
        var apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY")!;
        var apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")!;


        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<(string Url, string PublicId)> UploadAsync(
        IFormFile file, MediaType mediaType, string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
            throw new ArgumentException("La carpeta de destino es obligatoria.");

        await using var stream = file.OpenReadStream();

        if (mediaType == MediaType.Image)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error is not null)
                throw new InvalidOperationException(
                    $"Error al subir imagen a Cloudinary: {result.Error.Message}");

            return (result.SecureUrl.ToString(), result.PublicId);
        }
        else
        {
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error is not null)
                throw new InvalidOperationException(
                    $"Error al subir video a Cloudinary: {result.Error.Message}");

            return (result.SecureUrl.ToString(), result.PublicId);
        }
    }

    public async Task DeleteAsync(string publicId, MediaType mediaType)
    {
        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = mediaType == MediaType.Video
                ? ResourceType.Video
                : ResourceType.Image
        };

        var result = await _cloudinary.DestroyAsync(deleteParams);

        if (result.Error is not null)
            throw new InvalidOperationException(
                $"Error al eliminar archivo de Cloudinary: {result.Error.Message}");
    }
}