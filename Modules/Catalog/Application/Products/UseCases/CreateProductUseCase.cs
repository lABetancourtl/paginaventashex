using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Variants.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;

/// <summary> 
/// Caso de uso para crear un nuevo producto.
/// </summary> 
public class CreateProductUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductVariantRepository _variantRepository;

    public CreateProductUseCase(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IProductVariantRepository variantRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _variantRepository = variantRepository;
    }

    /// <summary>
    /// Ejecuta el caso de uso para crear un nuevo producto.
    /// </summary>
    /// <param name="dto">Datos del producto a crear son: name, description, price, sku, stock y category id.</param>
    /// <returns>Id del producto creado.</returns>
    public async Task<int> ExecuteAsync(CreateProductDto dto)
    {
        var category = await _categoryRepository.GetEntityByIdAsync(dto.CategoryId);

        if (category is null)
            throw new InvalidOperationException(
                $"No existe una categoría con Id '{dto.CategoryId}'.");

        if (category.ParentCategoryId is null)
            throw new ArgumentException(
                "Los productos solo se pueden crear en una subcategoría, no en una categoría principal.");

        if (!category.IsActive)
            throw new ArgumentException(
                "La subcategoría está desactivada. Actívala antes de crear productos en ella.");

        var parent = await _categoryRepository.GetEntityByIdAsync(category.ParentCategoryId.Value);

        if (parent is null || !parent.IsActive)
            throw new ArgumentException(
                "La categoría principal está desactivada. Actívala antes de crear productos en ella.");

        var sku = dto.Sku.Trim().ToUpper();

        if (await _variantRepository.ExistsBySkuAsync(sku))
            throw new InvalidOperationException(
                $"Ya existe un producto o una variante con el SKU '{sku}'.");

        var product = Product.Create(
            dto.Name,
            dto.Description,
            dto.Price,
            dto.CategoryId);

        product.AddDefaultVariant(sku, dto.Stock);

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return product.Id;
    }

}