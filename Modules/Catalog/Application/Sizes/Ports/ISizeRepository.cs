using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Ports;

public interface ISizeRepository
{
    Task AddAsync(Size size);
    Task<Size?> GetEntityByIdAsync(int id);
    Task<List<SizeResponseDto>> GetAllAsync(SizeGroup? sizeGroup, bool includeInactive);
    Task<bool> ExistsByNameAsync(SizeGroup sizeGroup, string name, int? excludeId = null);
    Task<int> GetNextDisplayOrderAsync(SizeGroup sizeGroup);
    Task<Size?> GetSiblingByDisplayOrderAsync(SizeGroup sizeGroup, int displayOrder);
    Task SaveChangesAsync();
}