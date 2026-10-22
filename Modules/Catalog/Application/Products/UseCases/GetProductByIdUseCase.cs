using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;

/// <summary>
/// Caso de uso para obtener el detalle de un producto por su Id.
/// Con onlyVisible = true (tienda pública) solo devuelve productos visibles en el catálogo:
/// producto activo, y su subcategoría y su raíz también activas.
/// Con onlyVisible = false (panel admin) devuelve cualquier producto.
/// </summary>
public class GetProductByIdUseCase
{
    private readonly IProductRepository _repository;

    public GetProductByIdUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductResponseDto> ExecuteAsync(int id, bool onlyVisible)
    {
        var product = await _repository.GetByIdAsync(id, onlyVisible);

        if (product is null)
            throw new AppException("PRODUCT_NOT_FOUND",
                $"No se encontró el producto con Id {id}.", 404);

        return product;
    }
}