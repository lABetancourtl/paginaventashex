using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;
using PaginaVentasNet.Api.Modules.Home.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Lista los medios de un banner, en orden.
/// </summary>
public class GetHomeSectionItemsUseCase
{
    private readonly IHomeSectionRepository _sections;
    private readonly IHomeSectionItemRepository _items;

    public GetHomeSectionItemsUseCase(
        IHomeSectionRepository sections,
        IHomeSectionItemRepository items)
    {
        _sections = sections;
        _items = items;
    }

    public async Task<List<HomeSectionItemResponseDto>> ExecuteAsync(int sectionId)
    {
        var section = await _sections.GetEntityByIdAsync(sectionId);

        if (section is null)
            throw new AppException("HOME_SECTION_NOT_FOUND",
                $"No se encontró la sección con Id {sectionId}.", 404);

        if (section.Type != HomeSectionType.Banner)
            throw new AppException("HOME_SECTION_INVALID_TYPE",
                "Solo los banners tienen medios.", 400);

        return await _items.GetDtosBySectionIdAsync(sectionId);
    }
}