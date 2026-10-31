using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.UseCases;

/// <summary>
/// Mueve una talla a un nuevo orden dentro de su grupo. La talla que ocupa el orden destino
/// toma el orden que dejó la movida (intercambio).
/// </summary>
public class ReorderSizeUseCase
{
    private readonly ISizeRepository _repository;

    public ReorderSizeUseCase(ISizeRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(int id, ReorderSizeDto dto)
    {
        var size = await _repository.GetEntityByIdAsync(id);

        if (size is null)
            throw new AppException("SIZE_NOT_FOUND",
                $"No se encontró la talla con Id {id}.", 404);

        if (size.DisplayOrder == dto.NewDisplayOrder)
            throw new AppException("SIZE_REORDER_INVALID",
                "La talla ya tiene ese orden.", 400);

        var sibling = await _repository.GetSiblingByDisplayOrderAsync(
            size.SizeGroup, dto.NewDisplayOrder);

        if (sibling is null)
            throw new AppException("SIZE_REORDER_INVALID",
                $"No existe ninguna talla del grupo {size.SizeGroup} con el orden {dto.NewDisplayOrder}.", 400);

        var originalOrder = size.DisplayOrder;

        sibling.SetDisplayOrder(originalOrder);
        size.SetDisplayOrder(dto.NewDisplayOrder);

        await _repository.SaveChangesAsync();
    }
}