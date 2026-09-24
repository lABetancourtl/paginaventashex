using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Categories;

/// <summary> 
/// Repositorio para la gestión de categorías.
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary> 
    /// Agrega una nueva categoría.
    /// </summary>
    /// <param name="category">Categoría a agregar.</param>
    /// <returns>Tarea que representa la operación de adición.</returns>
    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
    }

    /// <summary>
    /// Verifica si una categoría existe por su slug.
    /// </summary>
    /// <param name="slug">Slug de la categoría a verificar.</param>
    /// <returns>True si la categoría existe, false en caso contrario.</returns>
    public async Task<bool> ExistsBySlugAsync(string slug)
    {
        return await _context.Categories
            .AnyAsync(c => c.Slug == slug);
    }

    /// <summary>
    /// Guarda los cambios en la base de datos.
    /// </summary>
    /// <returns>Tarea que representa la operación de guardado.</returns>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene todas las categorías.
    /// </summary>
    /// <returns>Lista de categorías.</returns>
    public async Task<List<CategoryResponseDto>> GetAllAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                ParentCategoryId = c.ParentCategoryId,
                IsActive = c.IsActive,
                CreatedAtUtc = c.CreatedAtUtc
            })
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si una categoría existe por su Id.
    /// </summary>
    /// <param name="id">Id de la categoría a verificar.</param>
    /// <returns>True si la categoría existe, false en caso contrario.</returns>
    public async Task<bool> ExistsByIdAsync(int id)
    {
        return await _context.Categories
            .AnyAsync(c => c.Id == id);
    }

    /// <summary>
    /// Obtiene una categoría por su Id.
    /// </summary>
    /// <param name="id">Id de la categoría a obtener.</param>
    /// <returns>DTO de la categoría o null si no existe.</returns>
    public async Task<CategoryResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .Where(c => c.Id == id)
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                ParentCategoryId = c.ParentCategoryId,
                IsActive = c.IsActive,
                CreatedAtUtc = c.CreatedAtUtc
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Obtiene una categoría por su Id.
    /// </summary>
    /// <param name="id">Id de la categoría a obtener.</param>
    /// <returns>Entidad de la categoría o null si no existe.</returns>
    public async Task<Category?> GetEntityByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    /// <summary>
    /// Actualiza una categoría.
    /// </summary>
    /// <param name="category">Categoría a actualizar.</param>
    public  Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Obtiene todas las categorías activas organizadas en estructura jerárquica.
    /// Carga todas las categorías en memoria y construye el árbol recursivamente.
    /// </summary>
    public async Task<List<CategoryTreeDto>> GetTreeAsync()
    {
        var todasLasCategorias = await _context.Categories
        .Where(c => c.IsActive)
        .OrderBy(c => c.Name)
        .ToListAsync();

        return todasLasCategorias
        .Where(c => c.ParentCategoryId == null)
        .Select(c => MapToTree(c, todasLasCategorias))
        .ToList();
    }

    /// <summary>
    /// Mapea una categoría y sus hijos recursivamente a CategoryTreeDto.
    /// </summary>
    private static CategoryTreeDto MapToTree(Category category, List<Category> todas)
    {
        return new CategoryTreeDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            ParentCategoryId = category.ParentCategoryId,
            IsActive = category.IsActive,
            Children = todas
                .Where(c => c.ParentCategoryId == category.Id)
                .OrderBy(c => c.Name)
                .Select(c => MapToTree(c, todas))
                .ToList()
        };
}

}