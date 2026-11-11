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

    public HomeSectionItemController(
        GetHomeSectionItemsUseCase getItemsUseCase,
        AddHomeSectionItemUseCase addItemUseCase,
        UpdateHomeSectionItemUseCase updateItemUseCase,
        ReorderHomeSectionItemUseCase reorderItemUseCase,
        DeleteHomeSectionItemUseCase deleteItemUseCase)
    {
        _getItemsUseCase = getItemsUseCase;
        _addItemUseCase = addItemUseCase;
        _updateItemUseCase = updateItemUseCase;
        _reorderItemUseCase = reorderItemUseCase;
        _deleteItemUseCase = deleteItemUseCase;
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
}