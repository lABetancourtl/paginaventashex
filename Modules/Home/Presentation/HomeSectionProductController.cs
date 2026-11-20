using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.UseCases;

namespace PaginaVentasNet.Api.Modules.Home.Presentation;

/// <summary>
/// Administración de los productos de un estante de la página de inicio. Solo Admin.
/// </summary>
[Authorize(Roles = "Admin")]
[Route("api/home/sections/{sectionId:int}/products")]
public class HomeSectionProductController : ApiController
{
    private readonly GetHomeSectionProductsUseCase _getProductsUseCase;
    private readonly SetHomeSectionProductsUseCase _setProductsUseCase;

    public HomeSectionProductController(
        GetHomeSectionProductsUseCase getProductsUseCase,
        SetHomeSectionProductsUseCase setProductsUseCase)
    {
        _getProductsUseCase = getProductsUseCase;
        _setProductsUseCase = setProductsUseCase;
    }

    /// <summary>
    /// Lista los productos del estante, en orden. Cada uno trae isVisible: si es false,
    /// el producto no se muestra en la página de inicio (está desactivado o su categoría lo está).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<HomeSectionProductResponseDto>>>> GetAll(int sectionId)
    {
        try
        {
            var products = await _getProductsUseCase.ExecuteAsync(sectionId);
            return Success(products);
        }
        catch (AppException ex)
        {
            return Failure<List<HomeSectionProductResponseDto>>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Reemplaza la lista de productos del estante. El orden de la lista es el orden del estante.
    /// Hasta 4 productos, sin repetir. Una lista vacía deja el estante sin productos.
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<ApiResponse<bool>>> Set(int sectionId, SetHomeSectionProductsDto dto)
    {
        try
        {
            await _setProductsUseCase.ExecuteAsync(sectionId, dto);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }
}