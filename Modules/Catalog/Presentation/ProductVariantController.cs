using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.UseCases;

namespace PaginaVentasNet.Api.Modules.Catalog.Presentation;

/// <summary>
/// Administración de las variantes (talla, color, SKU y stock) de un producto. Solo Admin.
/// </summary>
[Authorize(Roles = "Admin")]
[Route("api/products/{productId:int}/variants")]
public class ProductVariantController : ApiController
{
    private readonly GetProductVariantsUseCase _getVariantsUseCase;
    private readonly CreateProductVariantUseCase _createVariantUseCase;
    private readonly UpdateProductVariantUseCase _updateVariantUseCase;
    private readonly ChangeProductVariantStatusUseCase _changeStatusUseCase;

    public ProductVariantController(
        GetProductVariantsUseCase getVariantsUseCase,
        CreateProductVariantUseCase createVariantUseCase,
        UpdateProductVariantUseCase updateVariantUseCase,
        ChangeProductVariantStatusUseCase changeStatusUseCase)
    {
        _getVariantsUseCase = getVariantsUseCase;
        _createVariantUseCase = createVariantUseCase;
        _updateVariantUseCase = updateVariantUseCase;
        _changeStatusUseCase = changeStatusUseCase;
    }

    /// <summary>
    /// Lista todas las variantes de un producto, activas o no.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductVariantResponseDto>>>> GetAll(int productId)
    {
        try
        {
            var variants = await _getVariantsUseCase.ExecuteAsync(productId);
            return Success(variants);
        }
        catch (AppException ex)
        {
            return Failure<List<ProductVariantResponseDto>>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Agrega una variante al producto. Talla y color son opcionales y deben ser
    /// coherentes con las demás variantes activas.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<int>>> Create(int productId, CreateProductVariantDto dto)
    {
        try
        {
            var id = await _createVariantUseCase.ExecuteAsync(productId, dto);
            return Success(id, 201);
        }
        catch (AppException ex)
        {
            return Failure<int>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Edita una variante. Reemplaza la variante completa: si no se envía sizeId o colorId,
    /// ese atributo queda en null. Los atributos solo cambian si es la única variante del producto.
    /// </summary>
    [HttpPut("{variantId:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        int productId, int variantId, UpdateProductVariantDto dto)
    {
        try
        {
            await _updateVariantUseCase.ExecuteAsync(productId, variantId, dto);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Activa una variante desactivada.
    /// </summary>
    [HttpPatch("{variantId:int}/activate")]
    public async Task<ActionResult<ApiResponse<bool>>> Activate(int productId, int variantId)
    {
        try
        {
            await _changeStatusUseCase.ActivateAsync(productId, variantId);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Desactiva una variante. Deja de poder comprarse, pero no se borra.
    /// </summary>
    [HttpPatch("{variantId:int}/deactivate")]
    public async Task<ActionResult<ApiResponse<bool>>> Deactivate(int productId, int variantId)
    {
        try
        {
            await _changeStatusUseCase.DeactivateAsync(productId, variantId);
            return Success(true);
        }
        catch (AppException ex)
        {
            return Failure<bool>(ex.Code, ex.Message, ex.StatusCode);
        }
    }
}