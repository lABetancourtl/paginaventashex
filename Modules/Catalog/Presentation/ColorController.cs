using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.UseCases;

namespace PaginaVentasNet.Api.Modules.Catalog.Presentation;

/// <summary>
/// Administración de colores. Solo Admin.
/// </summary>
[Authorize(Roles = "Admin")]
[Route("api/colors")]
public class ColorController : ApiController
{
    private readonly GetColorsUseCase _getColorsUseCase;
    private readonly CreateColorUseCase _createColorUseCase;
    private readonly UpdateColorUseCase _updateColorUseCase;
    private readonly ChangeColorStatusUseCase _changeColorStatusUseCase;

    public ColorController(
        GetColorsUseCase getColorsUseCase,
        CreateColorUseCase createColorUseCase,
        UpdateColorUseCase updateColorUseCase,
        ChangeColorStatusUseCase changeColorStatusUseCase)
    {
        _getColorsUseCase = getColorsUseCase;
        _createColorUseCase = createColorUseCase;
        _updateColorUseCase = updateColorUseCase;
        _changeColorStatusUseCase = changeColorStatusUseCase;
    }

    /// <summary>
    /// Lista los colores ordenados por nombre. Por defecto solo los activos.
    /// </summary>
    /// <param name="includeInactive">true para incluir los desactivados.</param>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ColorResponseDto>>>> GetAll(
        [FromQuery] bool includeInactive = false)
    {
        var colors = await _getColorsUseCase.ExecuteAsync(includeInactive);
        return Success(colors);
    }

    /// <summary>
    /// Crea un color. El código hexadecimal (#RRGGBB) es opcional.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<int>>> Create(CreateColorDto dto)
    {
        try
        {
            var id = await _createColorUseCase.ExecuteAsync(dto);
            return Success(id, 201);
        }
        catch (AppException ex)
        {
            return Failure<int>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Edita el nombre y el código de un color. Reemplaza el color completo:
    /// si no se envía hexCode, el color queda sin muestra.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(int id, UpdateColorDto dto)
    {
        try
        {
            await _updateColorUseCase.ExecuteAsync(id, dto);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Activa un color desactivado.
    /// </summary>
    [HttpPatch("{id:int}/activate")]
    public async Task<ActionResult<ApiResponse<bool>>> Activate(int id)
    {
        try
        {
            await _changeColorStatusUseCase.ActivateAsync(id);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Desactiva un color. Deja de ofrecerse, pero las variantes que ya lo usan siguen funcionando.
    /// </summary>
    [HttpPatch("{id:int}/deactivate")]
    public async Task<ActionResult<ApiResponse<bool>>> Deactivate(int id)
    {
        try
        {
            await _changeColorStatusUseCase.DeactivateAsync(id);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }
}