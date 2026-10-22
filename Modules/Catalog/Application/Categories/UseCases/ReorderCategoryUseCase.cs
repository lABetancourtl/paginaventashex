using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Categories.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Categories.UseCases;

/// <summary>
/// Caso de uso para mover una categoría a un nuevo orden entre sus hermanas.
/// La categoría que ocupa el orden destino toma el orden que dejó la movida (intercambio).
/// </summary>
public class ReorderCategoryUseCase
{
    private readonly ICategoryRepository _repository;

    public ReorderCategoryUseCase(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(int id, ReorderCategoryDto dto)
    {
        var category = await _repository.GetEntityByIdAsync(id);

        if (category is null)
            throw new AppException("CATEGORY_NOT_FOUND",
                $"No se encontró la categoría con Id {id}.", 404);

        if (category.DisplayOrder == dto.NewDisplayOrder)
            throw new AppException("CATEGORY_REORDER_INVALID",
                "La categoría ya tiene ese orden.", 400);

        var sibling = await _repository.GetSiblingByDisplayOrderAsync(
            category.ParentCategoryId, dto.NewDisplayOrder);

        if (sibling is null)
            throw new AppException("CATEGORY_REORDER_INVALID",
                $"No existe ninguna categoría hermana con el orden {dto.NewDisplayOrder}.", 400);

        var originalOrder = category.DisplayOrder;

        sibling.SetDisplayOrder(originalOrder);
        category.SetDisplayOrder(dto.NewDisplayOrder);

        await _repository.SaveChangesAsync();
    }
}