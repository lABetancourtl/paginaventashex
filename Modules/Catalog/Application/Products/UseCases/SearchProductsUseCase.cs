using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;

/// <summary> 
/// Caso de uso para buscar productos en el catálogo, usado por Usuarios no administradores.
/// </summary>
public class SearchProductsUseCase
{
    private readonly IProductRepository _repository;
    private readonly ICategoryRepository _categoryRepository;

    public SearchProductsUseCase(
        IProductRepository repository,
        ICategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Ejecuta el caso de uso para buscar productos en el catálogo.
    /// Si se envía un categorySlug que no existe o está desactivada, lanza un 404.
    /// </summary>
    /// <param name="dto">Datos de búsqueda: texto, categorySlug, onSale y página.</param>
    /// <returns>Resultado paginado de productos que coinciden con los criterios de búsqueda.</returns>
    public async Task<PagedResultDto<ProductResponseDto>> ExecuteAsync(SearchProductsDto dto)
    {
        if (dto.Page < 1)
            dto.Page = 1;

        if (!string.IsNullOrWhiteSpace(dto.CategorySlug))
        {
            var slug = dto.CategorySlug.Trim().ToLower();

            var existe = await _categoryRepository.ExistsActiveBySlugAsync(slug);

            if (!existe)
                throw new AppException("CATEGORY_NOT_FOUND",
                    $"No existe la categoría '{dto.CategorySlug}'.", 404);
        }

        return await _repository.SearchAsync(dto);
    }
}