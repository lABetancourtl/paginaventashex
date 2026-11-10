using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.UseCases;

namespace PaginaVentasNet.Api.Modules.Home.Presentation;

/// <summary>
/// Administración de las secciones de la página de inicio. Solo Admin.
/// </summary>
[Authorize(Roles = "Admin")]
[Route("api/home/sections")]
public class HomeSectionController : ApiController
{
    private readonly GetHomeSectionsUseCase _getSectionsUseCase;
    private readonly CreateHomeSectionUseCase _createSectionUseCase;
    private readonly UpdateHomeSectionUseCase _updateSectionUseCase;
    private readonly ChangeHomeSectionStatusUseCase _changeStatusUseCase;
    private readonly ReorderHomeSectionUseCase _reorderSectionUseCase;
    private readonly DeleteHomeSectionUseCase _deleteSectionUseCase;

    public HomeSectionController(
        GetHomeSectionsUseCase getSectionsUseCase,
        CreateHomeSectionUseCase createSectionUseCase,
        UpdateHomeSectionUseCase updateSectionUseCase,
        ChangeHomeSectionStatusUseCase changeStatusUseCase,
        ReorderHomeSectionUseCase reorderSectionUseCase,
        DeleteHomeSectionUseCase deleteSectionUseCase)
    {
        _getSectionsUseCase = getSectionsUseCase;
        _createSectionUseCase = createSectionUseCase;
        _updateSectionUseCase = updateSectionUseCase;
        _changeStatusUseCase = changeStatusUseCase;
        _reorderSectionUseCase = reorderSectionUseCase;
        _deleteSectionUseCase = deleteSectionUseCase;
    }

    /// <summary>
    /// Lista todas las secciones de la página de inicio, activas o no, en orden.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<HomeSectionResponseDto>>>> GetAll()
    {
        var sections = await _getSectionsUseCase.ExecuteAsync();
        return Success(sections);
    }

    /// <summary>
    /// Crea una sección al final de la página. Nace inactiva.
    /// Un banner no lleva título ni botón "Ver todo".
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<int>>> Create(CreateHomeSectionDto dto)
    {
        try
        {
            var id = await _createSectionUseCase.ExecuteAsync(dto);
            return Success(id, 201);
        }
        catch (AppException ex)
        {
            return Failure<int>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Edita el título y el botón "Ver todo" de una sección de productos.
    /// Reemplaza ambos campos: si no se envían, quedan en null.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(int id, UpdateHomeSectionDto dto)
    {
        try
        {
            await _updateSectionUseCase.ExecuteAsync(id, dto);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Activa una sección: empieza a mostrarse en la página de inicio.
    /// </summary>
    [HttpPatch("{id:int}/activate")]
    public async Task<ActionResult<ApiResponse<bool>>> Activate(int id)
    {
        try
        {
            await _changeStatusUseCase.ActivateAsync(id);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Desactiva una sección: deja de mostrarse, pero conserva su configuración.
    /// </summary>
    [HttpPatch("{id:int}/deactivate")]
    public async Task<ActionResult<ApiResponse<bool>>> Deactivate(int id)
    {
        try
        {
            await _changeStatusUseCase.DeactivateAsync(id);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Mueve una sección a un nuevo orden, por intercambio con la que lo ocupa.
    /// </summary>
    [HttpPatch("{id:int}/reorder")]
    public async Task<ActionResult<ApiResponse<bool>>> Reorder(int id, ReorderHomeSectionDto dto)
    {
        try
        {
            await _reorderSectionUseCase.ExecuteAsync(id, dto);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Borra una sección con sus medios y sus productos. Las siguientes suben un puesto.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            await _deleteSectionUseCase.ExecuteAsync(id);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }
}