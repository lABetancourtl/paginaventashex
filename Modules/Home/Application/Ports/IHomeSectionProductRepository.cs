using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Domain;

namespace PaginaVentasNet.Api.Modules.Home.Application.Ports;

public interface IHomeSectionProductRepository
{
    /// <summary>Entidades del estante, para compararlas con la lista nueva.</summary>
    Task<List<HomeSectionProduct>> GetBySectionIdAsync(int sectionId);

    Task<List<HomeSectionProductResponseDto>> GetDtosBySectionIdAsync(int sectionId);

    /// <summary>De los ids recibidos, devuelve los que existen como productos.</summary>
    Task<List<int>> GetExistingProductIdsAsync(IEnumerable<int> productIds);

    Task AddAsync(HomeSectionProduct item);
    Task RemoveAsync(HomeSectionProduct item);
    Task SaveChangesAsync();
}