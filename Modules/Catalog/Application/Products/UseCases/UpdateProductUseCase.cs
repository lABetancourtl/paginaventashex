using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;


/// <summary>
/// Caso de uso para actualizar un producto existente.
/// </summary>
public class UpdateProductUseCase
{
    private readonly IProductRepository _productRepository;

    public UpdateProductUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>
    /// Ejecuta el caso de uso para actualizar un producto existente.
    /// </summary>
    /// <param name="id">Id del producto a actualizar.</param>
    /// <param name="dto">Nuevos datos del producto: nombre, descripción y precio.</param>
    /// <returns>True si se actualizó correctamente, false si no se encontró el producto.</returns>
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
    
    /// <summary>
    /// Activa un producto existente.
    /// </summary>
    /// <param name="id">Id del producto a activar.</param>
    /// <returns>True si se activó correctamente, false si no se encontró el producto.</returns>
    public async Task<bool> ActivateAsync(int id)
    {
        var product = await _productRepository.GetEntityByIdAsync(id);

        if (product is null)
            return false;

        product.Activate();
        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Desactiva un producto existente.
    /// </summary>
    /// <param name="id">Id del producto a desactivar.</param>
    /// <returns>True si se desactivó correctamente, false si no se encontró el producto.</returns>
    public async Task<bool> DeactivateAsync(int id)
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