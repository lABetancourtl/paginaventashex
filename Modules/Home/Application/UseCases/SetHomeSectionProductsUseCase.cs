using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;
using PaginaVentasNet.Api.Modules.Home.Domain;
using PaginaVentasNet.Api.Modules.Home.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Reemplaza la lista de productos de un estante. El orden de la lista es el orden del estante.
/// Todo se valida antes de escribir: si algo falla, no se guarda nada. Los productos que siguen
/// conservan su fila y solo cambian de orden; los que salen se quitan y los nuevos se agregan,
/// todo en un solo guardado.
/// </summary>
public class SetHomeSectionProductsUseCase
{
    private readonly IHomeSectionRepository _sections;
    private readonly IHomeSectionProductRepository _products;

    public SetHomeSectionProductsUseCase(
        IHomeSectionRepository sections,
        IHomeSectionProductRepository products)
    {
        _sections = sections;
        _products = products;
    }

    public async Task ExecuteAsync(int sectionId, SetHomeSectionProductsDto dto)
    {
        var section = await _sections.GetEntityByIdAsync(sectionId);

        if (section is null)
            throw new AppException("HOME_SECTION_NOT_FOUND",
                $"No se encontró la sección con Id {sectionId}.", 404);

        if (section.Type != HomeSectionType.Products)
            throw new AppException("HOME_SECTION_INVALID_TYPE",
                "Solo los estantes de productos tienen productos.", 400);

        var ids = dto.ProductIds;

        if (ids.Count > HomeSection.MaxProducts)
            throw new AppException("HOME_PRODUCTS_VALIDATION",
                $"Un estante admite hasta {HomeSection.MaxProducts} productos.", 400);

        if (ids.Any(id => id <= 0))
            throw new AppException("HOME_PRODUCTS_VALIDATION",
                "Todos los ids de producto deben ser mayores a cero.", 400);

        if (ids.Distinct().Count() != ids.Count)
            throw new AppException("HOME_PRODUCTS_VALIDATION",
                "No se puede repetir un producto en el mismo estante.", 400);

        var existingIds = await _products.GetExistingProductIdsAsync(ids);
        var missing = ids.Except(existingIds).ToList();

        if (missing.Count > 0)
            throw new AppException("HOME_PRODUCTS_VALIDATION",
                $"No existen los productos con Id: {string.Join(", ", missing)}.", 400);

        var current = await _products.GetBySectionIdAsync(sectionId);
        var currentById = current.ToDictionary(c => c.ProductId);

        foreach (var leaving in current.Where(c => !ids.Contains(c.ProductId)))
            await _products.RemoveAsync(leaving);

        for (var i = 0; i < ids.Count; i++)
        {
            var order = i + 1;

            if (currentById.TryGetValue(ids[i], out var kept))
                kept.SetDisplayOrder(order);
            else
                await _products.AddAsync(HomeSectionProduct.Create(sectionId, ids[i], order));
        }

        await _products.SaveChangesAsync();
    }
}