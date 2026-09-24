using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;

/// <summary> 
/// Interfaz que define las operaciones de acceso a datos para la entidad Category.
/// </summary>
public interface ICategoryRepository
{
    Task AddAsync(Category category);
    Task<bool> ExistsByIdAsync(int categoryId);
    Task<bool> ExistsBySlugAsync(string slug);
    Task<List<CategoryResponseDto>> GetAllAsync();
    Task<CategoryResponseDto?> GetByIdAsync(int id);
    Task<Category?> GetEntityByIdAsync(int id);
    Task SaveChangesAsync();
    Task UpdateAsync(Category category);
    Task<List<CategoryTreeDto>> GetTreeAsync();
}