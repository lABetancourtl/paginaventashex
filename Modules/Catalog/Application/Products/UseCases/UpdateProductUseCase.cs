using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;

public class UpdateProductUseCase
{
    private readonly IProductRepository _productRepository;

    public UpdateProductUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<bool> ExecuteAsync(int id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetEntityByIdAsync(id);

        if (product is null)
            return false;

        product.Update(dto.Name, dto.Description, dto.Price);

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return true;
    }

    internal async Task<bool> ActivateAsync(int id)
    {
        var product = await _productRepository.GetEntityByIdAsync(id);

        if (product is null)
            return false;

        product.Activate();
        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return true;
    }

    internal async Task<bool> DeactivateAsync(int id)
    {
        var product = await _productRepository.GetEntityByIdAsync(id);

        if (product is null)
            return false;

        product.Deactivate();
        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return true;
    }
}