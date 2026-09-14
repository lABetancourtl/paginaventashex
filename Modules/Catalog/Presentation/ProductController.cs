using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;

namespace PaginaVentasNet.Api.Modules.Catalog.Presentation;

[Route("api/products")]
public class ProductController : ApiController
{
    private readonly CreateProductUseCase _createProductUseCase;
    private readonly GetProductsUseCase _getProductsUseCase;

    public ProductController(
        CreateProductUseCase createProductUseCase,
        GetProductsUseCase getProductsUseCase)
    {
        _createProductUseCase = createProductUseCase;
        _getProductsUseCase = getProductsUseCase;
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
}