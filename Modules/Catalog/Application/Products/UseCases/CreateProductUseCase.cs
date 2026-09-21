using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;

public class CreateProductUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateProductUseCase(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<int> ExecuteAsync(CreateProductDto dto)
    {
        var categoryExists = await _categoryRepository.ExistsByIdAsync(dto.CategoryId);

        if (!categoryExists)
            throw new InvalidOperationException(
                $"No existe una categoría con Id '{dto.CategoryId}'.");

        var skuExists = await _productRepository.ExistsBySkuAsync(dto.Sku);

        if (skuExists)
            throw new InvalidOperationException(
                $"Ya existe un producto con el SKU '{dto.Sku}'.");

        var product = Product.Create(
            dto.Name,
            dto.Description,
            dto.Price,
            dto.Sku,
            dto.Stock,
            dto.CategoryId);

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return product.Id;
    }

}