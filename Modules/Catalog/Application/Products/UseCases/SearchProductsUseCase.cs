using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;

/// <summary> 
/// Caso de uso para buscar productos en el catálogo.
/// </summary>
public class SearchProductsUseCase
{
    private readonly IProductRepository _repository;

    public SearchProductsUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Ejecuta el caso de uso para buscar productos en el catálogo.
    /// </summary>
    /// <param name="dto">Datos de búsqueda: nombre o categoría, página.</param>
    /// <returns>Resultado paginado de productos que coinciden con los criterios de búsqueda.</returns>
    public async Task<PagedResultDto<ProductResponseDto>> ExecuteAsync(SearchProductsDto dto)
    {
        if (dto.Page < 1)
            dto.Page = 1;

        return await _repository.SearchAsync(dto);
    }
}