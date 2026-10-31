using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.UseCases;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Presentation;

/// <summary>
/// Administración de tallas. Solo Admin.
/// </summary>
[Authorize(Roles = "Admin")]
[Route("api/sizes")]
public class SizeController : ApiController
{
    private readonly GetSizesUseCase _getSizesUseCase;
    private readonly CreateSizeUseCase _createSizeUseCase;
    private readonly UpdateSizeUseCase _updateSizeUseCase;
    private readonly ChangeSizeStatusUseCase _changeSizeStatusUseCase;
    private readonly ReorderSizeUseCase _reorderSizeUseCase;

    public SizeController(
        GetSizesUseCase getSizesUseCase,
        CreateSizeUseCase createSizeUseCase,
        UpdateSizeUseCase updateSizeUseCase,
        ChangeSizeStatusUseCase changeSizeStatusUseCase,
        ReorderSizeUseCase reorderSizeUseCase)
    {
        _getSizesUseCase = getSizesUseCase;
        _createSizeUseCase = createSizeUseCase;
        _updateSizeUseCase = updateSizeUseCase;
        _changeSizeStatusUseCase = changeSizeStatusUseCase;
        _reorderSizeUseCase = reorderSizeUseCase;
    }

    /// <summary>
    /// Lista las tallas ordenadas por grupo y por orden. Por defecto solo las activas.
    /// </summary>
    /// <param name="sizeGroup">Opcional: Bebe, Nino o Adulto.</param>
    /// <param name="includeInactive">true para incluir las desactivadas.</param>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<SizeResponseDto>>>> GetAll(
        [FromQuery] SizeGroup? sizeGroup,
        [FromQuery] bool includeInactive = false)
    {
        var sizes = await _getSizesUseCase.ExecuteAsync(sizeGroup, includeInactive);
        return Success(sizes);
    }

    /// <summary>
    /// Crea una talla al final de su grupo.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<int>>> Create(CreateSizeDto dto)
    {
        try
        {
            var id = await _createSizeUseCase.ExecuteAsync(dto);
            return Success(id, 201);
        }
        catch (AppException ex)
        {
            return Failure<int>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Renombra una talla. El grupo no se puede cambiar.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(int id, UpdateSizeDto dto)
    {
        try
        {
            await _updateSizeUseCase.ExecuteAsync(id, dto);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Activa una talla desactivada.
    /// </summary>
    [HttpPatch("{id:int}/activate")]
    public async Task<ActionResult<ApiResponse<bool>>> Activate(int id)
    {
        try
        {
            await _changeSizeStatusUseCase.ActivateAsync(id);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Desactiva una talla. Deja de ofrecerse, pero las variantes que ya la usan siguen funcionando.
    /// </summary>
    [HttpPatch("{id:int}/deactivate")]
    public async Task<ActionResult<ApiResponse<bool>>> Deactivate(int id)
    {
        try
        {
            await _changeSizeStatusUseCase.DeactivateAsync(id);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Mueve una talla a un nuevo orden dentro de su grupo, por intercambio.
    /// </summary>
    [HttpPatch("{id:int}/reorder")]
    public async Task<ActionResult<ApiResponse<bool>>> Reorder(int id, ReorderSizeDto dto)
    {
        try
        {
            await _reorderSizeUseCase.ExecuteAsync(id, dto);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }
}