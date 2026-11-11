using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Domain;

namespace PaginaVentasNet.Api.Modules.Home.Application.Ports;

public interface IHomeSectionItemRepository
{
    Task AddAsync(HomeSectionItem item);
    Task<HomeSectionItem?> GetEntityByIdAsync(int id);

    /// <summary>Entidades de la sección, en orden. Se usan para contar, reordenar y borrar.</summary>
    Task<List<HomeSectionItem>> GetBySectionIdAsync(int sectionId);

    Task<List<HomeSectionItemResponseDto>> GetDtosBySectionIdAsync(int sectionId);
    Task RemoveAsync(HomeSectionItem item);
    Task SaveChangesAsync();
}