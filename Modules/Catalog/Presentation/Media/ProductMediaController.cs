using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.UseCases;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Presentation.Media;

/// <summary>
/// Controller para la gestión de archivos multimedia de productos.
/// </summary>
[Authorize]
[Route("api/products/{productId}/media")]
public class ProductMediaController : ApiController
{
    private readonly UploadProductMediaUseCase _uploadMediaUseCase;

    public ProductMediaController(UploadProductMediaUseCase uploadMediaUseCase)
    {
        _uploadMediaUseCase = uploadMediaUseCase;
    }

    /// <summary>
    /// Sube una imagen al producto. Máximo 9 imágenes por producto.
    /// </summary>
    /// <param name="productId">Id del producto.</param>
    /// <param name="file">Archivo de imagen a subir.</param>
    /// <param name="isMain">Indica si es la imagen principal del producto.</param>
    [HttpPost("images")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<ProductMediaResponseDto>>> UploadImage(
        int productId,
        IFormFile file,
        [FromForm] bool isMain = false)
    {
        try
        {
            var result = await _uploadMediaUseCase.ExecuteAsync(
                productId, file, MediaType.Image, isMain);
            return Success(result, 201);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<ProductMediaResponseDto>("MEDIA_CONFLICT", ex.Message, 409);
        }
    }

    /// <summary>
    /// Sube un video al producto. Solo se permite 1 video por producto.
    /// </summary>
    /// <param name="productId">Id del producto.</param>
    /// <param name="file">Archivo de video a subir.</param>
    [HttpPost("video")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<ProductMediaResponseDto>>> UploadVideo(
        int productId,
        IFormFile file)
    {
        try
        {
            var result = await _uploadMediaUseCase.ExecuteAsync(
                productId, file, MediaType.Video);
            return Success(result, 201);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<ProductMediaResponseDto>("MEDIA_CONFLICT", ex.Message, 409);
        }
    }
}