using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.UseCases;

namespace PaginaVentasNet.Api.Modules.Catalog.Presentation;

[Authorize]
[Route("api/categories")]
public class CategoryController : ApiController
{
    private readonly CreateCategoryUseCase _createCategoryUseCase;
    private readonly GetCategoriesUseCase _getCategoriesUseCase;

    public CategoryController(
        CreateCategoryUseCase createCategoryUseCase,
        GetCategoriesUseCase getCategoriesUseCase)
    {
        _createCategoryUseCase = createCategoryUseCase;
        _getCategoriesUseCase = getCategoriesUseCase;
    }

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

        [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryResponseDto>>>> GetAll()
    {
        var categories = await _getCategoriesUseCase.ExecuteAsync();
        return Success(categories);
    }
}