using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    Task<bool> ExistsBySlugAsync(string slug);
    Task<List<CategoryResponseDto>> GetAllAsync();
    Task SaveChangesAsync();

}