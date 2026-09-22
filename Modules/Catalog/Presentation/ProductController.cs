using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;

namespace PaginaVentasNet.Api.Modules.Catalog.Presentation;

/// <summary>
/// Controller para la gestión de productos del catálogo.
/// Requiere autenticación JWT para todos los endpoints.
/// </summary>
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

    /// <summary>
    /// Crea un nuevo producto en el catálogo.
    /// </summary>
    /// <param name="dto">Datos del producto: nombre, descripción, precio, SKU, stock y categoría.</param>
    /// <returns>Id del producto creado.</returns>
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

    /// <summary>
    /// Obtiene todos los productos activos del catálogo ordenados por nombre.
    /// </summary>
    /// <returns>Lista de productos activos.</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductResponseDto>>>> GetAll()
    {
        var products = await _getProductsUseCase.ExecuteAsync();
        return Success(products);
    }

    /// <summary>
    /// Obtiene todos los productos inactivos del catálogo ordenados por nombre.
    /// </summary>
    /// <returns>Lista de productos inactivos.</returns>
    [HttpGet("inactive")]
    public async Task<ActionResult<ApiResponse<List<ProductResponseDto>>>> GetAllDeactive()
    {
        var products = await _getProductsUseCase.ExecuteDeactiveAsync();
        return Success(products);
    }


    /// <summary>
    /// Obtiene un producto específico por su Id.
    /// </summary>
    /// <param name="id">Id del producto a buscar.</param>
    /// <returns>Datos del producto encontrado o 404 si no existe.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetById(int id)
    {
        var product = await _getProductsUseCase.ExecuteAsync(id);

        if (product == null)
            return Failure<ProductResponseDto>("PRODUCT_NOT_FOUND",
                $"No se encontró el producto con Id {id}.", 404);

        return Success(product);
    }

    /// <summary>
    /// Actualiza el nombre, descripción y precio de un producto existente.
    /// </summary>
    /// <param name="id">Id del producto a actualizar.</param>
    /// <param name="dto">Nuevos datos del producto.</param>
    /// <returns>True si se actualizó correctamente o 404 si no existe.</returns>
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

    /// <summary>
    /// Activa un producto para que sea visible en el catálogo público.
    /// </summary>
    /// <param name="id">Id del producto a activar.</param>
    /// <returns>True si se activó correctamente, 400 si ya estaba activo o 404 si no existe.</returns>
    [HttpPatch("{id}/activate")]
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
        var result = await _searchProductsUseCase.ExecuteAsync(dto);
        return Success(result);
    }
}