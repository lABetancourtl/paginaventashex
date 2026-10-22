using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;
using PaginaVentasNet.Api.Common.Exceptions;

namespace PaginaVentasNet.Api.Modules.Catalog.Presentation;

/// <summary>
/// Controller para la gestión de productos del catálogo.
/// Requiere autenticación JWT para todos los endpoints.
/// </summary>
[Route("api/products")]
public class ProductController : ApiController
{
    private readonly CreateProductUseCase _createProductUseCase;
    private readonly UpdateProductUseCase _updateProductUseCase;
    private readonly SearchProductsUseCase _searchProductsUseCase;
    private readonly AdminSearchProductsUseCase _adminSearchProductsUseCase;
    private readonly GetProductByIdUseCase _getProductByIdUseCase;

    public ProductController(
        CreateProductUseCase createProductUseCase,
        UpdateProductUseCase updateProductUseCase,
        SearchProductsUseCase searchProductsUseCase,
        AdminSearchProductsUseCase adminSearchProductsUseCase,
        GetProductByIdUseCase getProductByIdUseCase)
    {
        _createProductUseCase = createProductUseCase;
        _updateProductUseCase = updateProductUseCase;
        _searchProductsUseCase = searchProductsUseCase;
        _adminSearchProductsUseCase = adminSearchProductsUseCase;
        _getProductByIdUseCase = getProductByIdUseCase;
    }

    /// <summary>
    /// <param name="dto">Datos del producto: nombre, descripción, precio, SKU, stock y categoría.</param>
    /// <returns>Id del producto creado.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
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

    /// <summary>
    /// Actualiza el nombre, descripción y precio de un producto existente.
    /// </summary>
    /// <param name="id">Id del producto a actualizar.</param>
    /// <param name="dto">Nuevos datos del producto.</param>
    /// <returns>True si se actualizó correctamente o 404 si no existe.</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
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

    /// <summary>
    /// Activa un producto para que sea visible en el catálogo público.
    /// </summary>
    /// <param name="id">Id del producto a activar.</param>
    /// <returns>True si se activó correctamente, 400 si ya estaba activo o 404 si no existe.</returns>
    [HttpPatch("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> Activate(int id)
    {
        try
        {
            var activated = await _updateProductUseCase.ActivateAsync(id);

            if (!activated)
                return Failure<bool>("PRODUCT_NOT_FOUND",
                    $"No se encontró el producto con Id {id}.", 404);

            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("PRODUCT_INVALID_STATE", ex.Message, 400);
        }
    }

    /// <summary>
    /// Desactiva un producto para ocultarlo del catálogo público (soft delete).
    /// El producto no se elimina de la base de datos.
    /// </summary>
    /// <param name="id">Id del producto a desactivar.</param>
    /// <returns>True si se desactivó correctamente, 400 si ya estaba inactivo o 404 si no existe.</returns>
    [HttpPatch("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
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

    /// <summary>
    /// Búsqueda de productos para el panel de administración.
    /// Permite filtrar por múltiples categorías, nombre y estado.
    /// Muestra 20 productos por página incluyendo activos e inactivos.
    /// </summary>
    /// <param name="dto">Filtros: search, categoryIds, isActive, page.</param>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PagedResultDto<ProductResponseDto>>>> AdminSearch(
        [FromQuery] AdminSearchProductsDto dto)
    {
        var result = await _adminSearchProductsUseCase.ExecuteAsync(dto);
        return Success(result);
    }

    /// <summary>
    /// Busca productos por nombre o categoría con paginación.
    /// Si no se especifica búsqueda, retorna todos los productos activos.
    /// Muestra 10 productos por página.
    /// </summary>
    /// <param name="dto">Parámetros de búsqueda: término opcional y número de página.</param>
    /// <returns>Lista paginada de productos con total de resultados y páginas.</returns>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResultDto<ProductResponseDto>>>> Search(
        [FromQuery] SearchProductsDto dto)
    {
        try
        {
            var result = await _searchProductsUseCase.ExecuteAsync(dto);
            return Success(result);
        }
        catch (AppException ex)
        {
            return Failure<PagedResultDto<ProductResponseDto>>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

        /// <summary>
    /// Detalle público de un producto, con todas sus imágenes y su video.
    /// Solo responde si el producto es visible en el catálogo: producto activo,
    /// y su subcategoría y su raíz también activas. Si no, responde 404.
    /// </summary>
    /// <param name="id">Id del producto.</param>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetById(int id)
    {
        try
        {
            var product = await _getProductByIdUseCase.ExecuteAsync(id, onlyVisible: true);
            return Success(product);
        }
        catch (AppException ex)
        {
            return Failure<ProductResponseDto>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Detalle de un producto para el panel admin (formulario de edición).
    /// Devuelve cualquier producto, esté activo o no. Solo Admin.
    /// </summary>
    /// <param name="id">Id del producto.</param>
    [HttpGet("admin/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> AdminGetById(int id)
    {
        try
        {
            var product = await _getProductByIdUseCase.ExecuteAsync(id, onlyVisible: false);
            return Success(product);
        }
        catch (AppException ex)
        {
            return Failure<ProductResponseDto>(ex.Code, ex.Message, ex.StatusCode);
        }
    }

    /// <summary>
    /// Aplica un descuento a un producto existente. Solo Admin.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("{id}/discount")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> ApplyDiscount(int id, ApplyDiscountDto dto)
    {
        try
        {
            var applied = await _updateProductUseCase.ApplyDiscountAsync(id, dto.DiscountPercent);

            if (!applied)
                return Failure<bool>("PRODUCT_NOT_FOUND",
                    $"No se encontró el producto con Id {id}.", 404);

            return Success(true);
        }
        catch (ArgumentException ex)
        {
            return Failure<bool>("DISCOUNT_VALIDATION", ex.Message, 400);
        }
    }

    /// <summary>
    /// Elimina el descuento aplicado a un producto existente. Solo Admin.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}/discount")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveDiscount(int id)
    {
        try
        {
            var removed = await _updateProductUseCase.RemoveDiscountAsync(id);

            if (!removed)
                return Failure<bool>("PRODUCT_NOT_FOUND",
                    $"No se encontró el producto con Id {id}.", 404);

            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("DISCOUNT_INVALID_STATE", ex.Message, 400);
        }
    }
}