using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Ports;

public interface IColorRepository
{
    Task AddAsync(Color color);
    Task<Color?> GetEntityByIdAsync(int id);
    Task<List<ColorResponseDto>> GetAllAsync(bool includeInactive);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task SaveChangesAsync();
}