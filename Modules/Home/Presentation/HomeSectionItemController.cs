using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.UseCases;

namespace PaginaVentasNet.Api.Modules.Home.Presentation;

/// <summary>
/// Administración de los medios (imágenes y videos) de un banner de la página de inicio. Solo Admin.
/// </summary>
[Authorize(Roles = "Admin")]
[Route("api/home/sections/{sectionId:int}/items")]
public class HomeSectionItemController : ApiController
{
    // 50 MB de video más el sobrecosto del formulario multipart
    private const long MaxUploadRequestBytes = 55L * 1024 * 1024;

    private readonly GetHomeSectionItemsUseCase _getItemsUseCase;
    private readonly AddHomeSectionItemUseCase _addItemUseCase;
    private readonly UpdateHomeSectionItemUseCase _updateItemUseCase;
    private readonly ReorderHomeSectionItemUseCase _reorderItemUseCase;
    private readonly DeleteHomeSectionItemUseCase _deleteItemUseCase;
    private readonly SetHomeSectionItemMobileUseCase _setMobileUseCase;
    private readonly DeleteHomeSectionItemMobileUseCase _deleteMobileUseCase;

    public HomeSectionItemController(
        GetHomeSectionItemsUseCase getItemsUseCase,
        AddHomeSectionItemUseCase addItemUseCase,
        UpdateHomeSectionItemUseCase updateItemUseCase,
        ReorderHomeSectionItemUseCase reorderItemUseCase,
        DeleteHomeSectionItemUseCase deleteItemUseCase,
        SetHomeSectionItemMobileUseCase setMobileUseCase,
        DeleteHomeSectionItemMobileUseCase deleteMobileUseCase)
    {
        _getItemsUseCase = getItemsUseCase;
        _addItemUseCase = addItemUseCase;
        _updateItemUseCase = updateItemUseCase;
        _reorderItemUseCase = reorderItemUseCase;
        _deleteItemUseCase = deleteItemUseCase;
        _setMobileUseCase = setMobileUseCase;
        _deleteMobileUseCase = deleteMobileUseCase;
    }

    /// <summary>
    /// Lista los medios de un banner, en orden.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<HomeSectionItemResponseDto>>>> GetAll(int sectionId)
    {
        try
        {
            var items = await _getItemsUseCase.ExecuteAsync(sectionId);
            return Success(items);
        }
        catch (AppException ex)
        {
            return Failure<List<HomeSectionItemResponseDto>>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Agrega una imagen o un video al final del banner. multipart/form-data con el campo
    /// file y, opcionalmente, categoryId para que el medio enlace a una categoría.
    /// El tipo se deduce del archivo.
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxUploadRequestBytes)]
    public async Task<ActionResult<ApiResponse<HomeSectionItemResponseDto>>> Add(
        int sectionId,
        [FromForm] AddHomeSectionItemDto dto)
    {
        try
        {
            var item = await _addItemUseCase.ExecuteAsync(sectionId, dto);
            return Success(item, 201);
        }
        catch (AppException ex)
        {
            return Failure<HomeSectionItemResponseDto>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Cambia el enlace de un medio. Reemplaza el enlace: sin categoryId, el medio deja de ser un enlace.
    /// </summary>
    [HttpPut("{itemId:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int sectionId, int itemId, UpdateHomeSectionItemDto dto)
    {
        try
        {
            await _updateItemUseCase.ExecuteAsync(sectionId, itemId, dto);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Mueve un medio a un nuevo orden dentro del banner, por intercambio con el que lo ocupa.
    /// </summary>
    [HttpPatch("{itemId:int}/reorder")]
    public async Task<ActionResult<ApiResponse<bool>>> Reorder(
        int sectionId, int itemId, ReorderHomeSectionItemDto dto)
    {
        try
        {
            await _reorderItemUseCase.ExecuteAsync(sectionId, itemId, dto);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Quita un medio del banner y borra su archivo de Cloudinary. Los siguientes suben un puesto.
    /// </summary>
    [HttpDelete("{itemId:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int sectionId, int itemId)
    {
        try
        {
            await _deleteItemUseCase.ExecuteAsync(sectionId, itemId);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Pone o reemplaza la imagen para celular (se entrega recortada a 4:5) del medio.
    /// Solo si el banner tiene un único medio. multipart/form-data con el campo file.
    /// </summary>
    [HttpPut("{itemId:int}/mobile")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxUploadRequestBytes)]
    public async Task<ActionResult<ApiResponse<HomeSectionItemResponseDto>>> SetMobile(
        int sectionId, int itemId, [FromForm] SetHomeSectionItemMobileDto dto)
    {
        try
        {
            var item = await _setMobileUseCase.ExecuteAsync(sectionId, itemId, dto);
            return Success(item);
        }
        catch (AppException ex)
        {
            return Failure<HomeSectionItemResponseDto>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Quita la imagen para celular del medio. En celular se vuelve a ver la imagen principal.
    /// </summary>
    [HttpDelete("{itemId:int}/mobile")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteMobile(int sectionId, int itemId)
    {
        try
        {
            await _deleteMobileUseCase.ExecuteAsync(sectionId, itemId);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }
}
