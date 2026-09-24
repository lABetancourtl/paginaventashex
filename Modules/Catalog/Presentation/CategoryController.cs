using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.UseCases;

namespace PaginaVentasNet.Api.Modules.Catalog.Presentation;

/// <summary>
/// Controlador para la gestión de categorías.
/// </summary>
[Authorize]
[Route("api/categories")]
public class CategoryController : ApiController
{
    private readonly CreateCategoryUseCase _createCategoryUseCase;
    private readonly GetCategoriesUseCase _getCategoriesUseCase;
    private readonly UpdateCategoryUseCase _updateCategoryUseCase;
    private readonly GetCategoryTreeUseCase _getCategoryTreeUseCase;

    public CategoryController(
        CreateCategoryUseCase createCategoryUseCase,
        GetCategoriesUseCase getCategoriesUseCase,
        UpdateCategoryUseCase updateCategoryUseCase,
        GetCategoryTreeUseCase getCategoryTreeUseCase)
    {
        _createCategoryUseCase = createCategoryUseCase;
        _getCategoriesUseCase = getCategoriesUseCase;
        _updateCategoryUseCase = updateCategoryUseCase;
        _getCategoryTreeUseCase = getCategoryTreeUseCase;
    }

    /// <summary> 
    /// Crea una nueva categoría.
    /// </summary>
    /// <param name="dto">Datos de la categoría a crear.</param>
    /// <returns>El Id de la categoría creada o un error si ya existe.</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<int>>> Create(CreateCategoryDto dto)
    {
        try
        {
            var id = await _createCategoryUseCase.ExecuteAsync(dto);
            return Success(id, 201);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<int>("CATEGORY_CONFLICT", ex.Message, 409);
        }
        catch (ArgumentException ex)
        {
            return Failure<int>("CATEGORY_VALIDATION", ex.Message, 400);
        }
    }

    /// <summary>
    /// Obtiene todas las categorías.
    /// </summary>
    /// <returns>Lista de categorías.</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryResponseDto>>>> GetAll()
    {
        var categories = await _getCategoriesUseCase.ExecuteAsync();
        return Success(categories);
    }


    /// <summary>
    /// Obtiene una categoría por su Id.     
    /// </summary>
    /// <param name="id">Id de la categoría a buscar.</param>
    /// <returns>Datos de la categoría o 404 si no existe.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> GetById(int id)
    {
        var category = await _getCategoriesUseCase.ExecuteAsync(id);

        if (category == null)
            return Failure<CategoryResponseDto>("CATEGORY_NOT_FOUND",
                $"No se encontró la categoría con Id {id}.", 404);

        return Success(category);
    }

    /// <summary>
    /// Actualiza una categoría existente por su Id, su nombre y su slug.
    /// </summary>
    /// <param name="id">Id de la categoría a actualizar.</param>
    /// <param name="dto">Datos de la categoría a actualizar.</param>
    /// <returns>El Id de la categoría actualizada o un error si no se encuentra.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<int>>> Update(int id, UpdateCategoryDto dto)
    {
        try
        {
            var updatedId = await _updateCategoryUseCase.ExecuteAsync(id, dto);

            if (!updatedId)
                return Failure<int>("CATEGORY_NOT_FOUND",
                    $"No se encontró la categoría con Id {id}.", 404);

            return Success(id);
        }
        catch (ArgumentException ex)
        {
            return Failure<int>("CATEGORY_VALIDATION", ex.Message, 400);
        }
    }

    /// <summary>
    /// Activa una categoría existente por su Id.
    /// </summary> 
    /// <param name="id">Id de la categoría a activar.</param>
    /// <returns>True si se activó correctamente, false si no se encontró la categoría con el Id proporcionado.</returns>
    [HttpPatch("{id}/activate")]
    public async Task<ActionResult<ApiResponse<bool>>> Activate(int id)
    {
        try
        {
            var activatedId = await _updateCategoryUseCase.ActivateAsync(id);
            
            if (!activatedId)
                return Failure<bool>("CATEGORY_NOT_FOUND",
                    $"No se encontró la categoría con Id {id}.", 404);
            
            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("CATEGORY_INVALID_STATE", ex.Message, 400);
        }
    }
  
    /// <summary>
    /// Desactiva una categoría existente por su Id.
    /// </summary>
    /// <param name="id">Id de la categoría a desactivar.</param>
    /// <returns>True si se desactivó correctamente, false si no se encontró la categoría con el Id proporcionado.</returns>
    [HttpPatch("{id}/deactivate")]
    public async Task<ActionResult<ApiResponse<bool>>> Deactivate(int id)
    {
        try
        {
            var deactivatedId = await _updateCategoryUseCase.DeactivateAsync(id);   

            if (!deactivatedId)
                return Failure<bool>("CATEGORY_NOT_FOUND",
                    $"No se encontró la categoría con Id {id}.", 404);
            
            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("CATEGORY_INVALID_STATE", ex.Message, 400);
        }
    }

    [HttpGet("tree")]
    public async Task<ActionResult<ApiResponse<List<CategoryTreeDto>>>> GetTree()
    {
        var tree = await _getCategoryTreeUseCase.ExecuteAsync();
        return Success(tree);
    }


    
}