using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    Task<bool> ExistsByIdAsync(int categoryId);
    Task<bool> ExistsBySlugAsync(string slug);
    Task<List<CategoryResponseDto>> GetAllAsync();
    Task SaveChangesAsync();

}