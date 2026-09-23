using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;

/// <summary> 
/// Caso de uso para obtener todos los productos del catálogo.
/// </summary>
public class GetProductsUseCase
{
    private readonly IProductRepository _repository;

    public GetProductsUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Ejecuta el caso de uso para obtener todos los productos del catálogo que esten activos.
    /// </summary>
    /// <returns>Lista de productos activos en el catálogo.</returns>  
    public async Task<List<ProductResponseDto>> ExecuteAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <summary>
    /// Ejecuta el caso de uso para obtener todos los productos del catálogo que esten desactivados.
    /// </summary>
    /// <returns>Lista de productos desactivados en el catálogo.</returns>
    public async Task<List<ProductResponseDto>> ExecuteDeactiveAsync()
    {
        return await _repository.GetDeactiveAsync();
    }

    /// <summary>
    /// Ejecuta el caso de uso para obtener un producto por su Id.
    /// </summary>
    /// <param name="id">Id del producto a obtener.</param>
    /// <returns>Producto que coincide con el Id proporcionado, o null si no se encuentra.</returns>
    public async Task<ProductResponseDto?> ExecuteByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }
}