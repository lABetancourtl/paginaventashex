using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;
using PaginaVentasNet.Api.Modules.Home.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Lista los productos de un estante, en orden, con su estado de visibilidad.
/// </summary>
public class GetHomeSectionProductsUseCase
{
    private readonly IHomeSectionRepository _sections;
    private readonly IHomeSectionProductRepository _products;

    public GetHomeSectionProductsUseCase(
        IHomeSectionRepository sections,
        IHomeSectionProductRepository products)
    {
        _sections = sections;
        _products = products;
    }

    public async Task<List<HomeSectionProductResponseDto>> ExecuteAsync(int sectionId)
    {
        var section = await _sections.GetEntityByIdAsync(sectionId);

        if (section is null)
            throw new AppException("HOME_SECTION_NOT_FOUND",
                $"No se encontró la sección con Id {sectionId}.", 404);

        if (section.Type != HomeSectionType.Products)
            throw new AppException("HOME_SECTION_INVALID_TYPE",
                "Solo los estantes de productos tienen productos.", 400);

        return await _products.GetDtosBySectionIdAsync(sectionId);
    }
}