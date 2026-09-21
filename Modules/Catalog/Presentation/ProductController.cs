using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;

namespace PaginaVentasNet.Api.Modules.Catalog.Presentation;

[Authorize]
[Route("api/products")]
public class ProductController : ApiController
{
    private readonly CreateProductUseCase _createProductUseCase;
    private readonly GetProductsUseCase _getProductsUseCase;
    private readonly UpdateProductUseCase _updateProductUseCase;
    private readonly SearchProductsUseCase _searchProductsUseCase;

    public ProductController(
        CreateProductUseCase createProductUseCase,
        GetProductsUseCase getProductsUseCase,
        UpdateProductUseCase updateProductUseCase,
        SearchProductsUseCase searchProductsUseCase)
    {
        _createProductUseCase = createProductUseCase;
        _getProductsUseCase = getProductsUseCase;
        _updateProductUseCase = updateProductUseCase;
        _searchProductsUseCase = searchProductsUseCase;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<int>>> Create(CreateProductDto dto)
    {
        try
        {
            var id = await _createProductUseCase.ExecuteAsync(dto);
            return Success(id, 201);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<int>("PRODUCT_CONFLICT", ex.Message, 409);
        }
        catch (ArgumentException ex)
        {
            return Failure<int>("PRODUCT_VALIDATION", ex.Message, 400);
        }
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductResponseDto>>>> GetAll()
    {
        var products = await _getProductsUseCase.ExecuteAsync();
        return Success(products);
    }

    [HttpGet("{id}")]
     public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetById(int id)
    {
        var product = await _getProductsUseCase.ExecuteAsync(id);

        if (product == null)
        {
            return Failure<ProductResponseDto>("PRODUCT_NOT_FOUND", 
            $"No se encontrol el producto con ID {id}.", 404);
        }
        return Success(product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(int id, UpdateProductDto dto)
    {
        try
        {
            var updated = await _updateProductUseCase.ExecuteAsync(id, dto);

            if (!updated)
                return Failure<bool>("PRODUCT_NOT_FOUND",
                    $"No se encontró el producto con Id {id}.", 404);

            return Success(true);
        }
        catch (ArgumentException ex)
        {
            return Failure<bool>("PRODUCT_VALIDATION", ex.Message, 400);
        }
    }

    [HttpPatch("{id}/activate")]
    public async Task<ActionResult<ApiResponse<bool>>> Activate(int id)
    {
        try
        {
            var deactivated = await _updateProductUseCase.ActivateAsync(id);

            if (!deactivated)
                return Failure<bool>("PRODUCT_NOT_FOUND",
                    $"No se encontró el producto con Id {id}.", 404);

            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("PRODUCT_INVALID_STATE", ex.Message, 400);
        }
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<ActionResult<ApiResponse<bool>>> Deactivate(int id)
    {
        try
        {
            var deactivated = await _updateProductUseCase.DeactivateAsync(id);

            if (!deactivated)
                return Failure<bool>("PRODUCT_NOT_FOUND",
                    $"No se encontró el producto con Id {id}.", 404);

            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("PRODUCT_INVALID_STATE", ex.Message, 400);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResultDto<ProductResponseDto>>>> Search(
        [FromQuery] SearchProductsDto dto)
    {
        var result = await _searchProductsUseCase.ExecuteAsync(dto);
        return Success(result);
    }

}