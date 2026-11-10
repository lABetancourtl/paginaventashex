using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Domain;

namespace PaginaVentasNet.Api.Modules.Home.Application.Ports;

public interface IHomeSectionRepository
{
    Task AddAsync(HomeSection section);
    Task<HomeSection?> GetEntityByIdAsync(int id);
    Task<List<HomeSectionResponseDto>> GetAllDtosAsync();
    Task<int> GetNextDisplayOrderAsync();
    Task<HomeSection?> GetByDisplayOrderAsync(int displayOrder);
    Task<List<HomeSection>> GetWithOrderGreaterThanAsync(int displayOrder);
    Task RemoveAsync(HomeSection section);
    Task SaveChangesAsync();
}