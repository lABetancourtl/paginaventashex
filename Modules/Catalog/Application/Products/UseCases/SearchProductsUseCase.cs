using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;

public class SearchProductsUseCase
{
    private readonly IProductRepository _repository;

    public SearchProductsUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<ProductResponseDto>> ExecuteAsync(SearchProductsDto dto)
    {
        if (dto.Page < 1)
            dto.Page = 1;

        return await _repository.SearchAsync(dto);
    }
}