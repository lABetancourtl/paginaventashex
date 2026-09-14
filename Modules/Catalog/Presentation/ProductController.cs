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

    public ProductController(CreateProductUseCase createProductUseCase)
    {
        _createProductUseCase = createProductUseCase;
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
}