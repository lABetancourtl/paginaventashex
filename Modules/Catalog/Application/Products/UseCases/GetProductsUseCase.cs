using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;

public class GetProductsUseCase
{
    private readonly IProductRepository _repository;

    public GetProductsUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductResponseDto>> ExecuteAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<List<ProductResponseDto>> ExecuteDeactiveAsync()
    {
        return await _repository.GetDeactiveAsync();
    }

    public async Task<ProductResponseDto?> ExecuteAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }
}