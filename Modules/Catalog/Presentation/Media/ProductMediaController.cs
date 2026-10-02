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
    private readonly GetProductMediaUseCase _getProductMediaUseCase;
    private readonly DeleteProductMediaUseCase _deleteProductMediaUseCase;

    public ProductMediaController(
        UploadProductMediaUseCase uploadMediaUseCase,
        GetProductMediaUseCase getProductMediaUseCase,
        DeleteProductMediaUseCase deleteProductMediaUseCase)
    {
        _uploadMediaUseCase = uploadMediaUseCase;
        _getProductMediaUseCase = getProductMediaUseCase;
        _deleteProductMediaUseCase = deleteProductMediaUseCase;
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
        IFormFile file)
    {
        try
        {
            var result = await _uploadMediaUseCase.ExecuteAsync(
                productId, file, MediaType.Image);
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

    /// <summary>
    /// Obtiene todos los archivos multimedia de un producto.
    /// </summary>
    /// <param name="productId">Id del producto.</param>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductMediaResponseDto>>>> GetAll(
        int productId)
    {
        try
        {
            var media = await _getProductMediaUseCase.ExecuteAsync(productId);
            return Success(media);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<List<ProductMediaResponseDto>>("PRODUCT_NOT_FOUND", ex.Message, 404);
        }
    }
    /// <summary>
    /// Elimina un archivo multimedia de un producto.      
    /// </summary>
    /// <param name="productId"> es el ID del producto.</param>
    /// <param name="mediaId"> es el ID del archivo multimedia.</param>
    /// <returns></returns>
    [HttpDelete("{mediaId}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(
        int productId,
        int mediaId)
    {
        try
        {
            var result = await _deleteProductMediaUseCase.DeleteAsync(productId, mediaId);
            return Success(result);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("MEDIA_NOT_FOUND", ex.Message, 404);
        }
    }

    /// <summary>
    /// Establece un archivo multimedia como principal.
    /// </summary>
    /// <param name="productId">Id del producto.</param>
    /// <param name="mediaId">Id del archivo multimedia.</param>
    /// <returns></returns>
    [HttpPatch("{mediaId}/main")]
    public async Task<ActionResult<ApiResponse<bool>>> SetAsMain(
        int productId,
        int mediaId)
    {
        try
        {
            var result = await _uploadMediaUseCase.SetAsMainAsync(productId, mediaId);
            return Success(result);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("MEDIA_NOT_FOUND", ex.Message, 404);
        }
        catch (ArgumentException ex)
        {
            return Failure<bool>("MEDIA_INVALID", ex.Message, 400);
        }
    }

}