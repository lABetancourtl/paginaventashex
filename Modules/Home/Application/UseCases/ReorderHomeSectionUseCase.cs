using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Mueve una sección a un nuevo orden. La sección que ocupa el orden destino
/// toma el orden que dejó la movida (intercambio).
/// </summary>
public class ReorderHomeSectionUseCase
{
    private readonly IHomeSectionRepository _repository;

    public ReorderHomeSectionUseCase(IHomeSectionRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(int id, ReorderHomeSectionDto dto)
    {
        var section = await _repository.GetEntityByIdAsync(id);

        if (section is null)
            throw new AppException("HOME_SECTION_NOT_FOUND",
                $"No se encontró la sección con Id {id}.", 404);

        if (section.DisplayOrder == dto.NewDisplayOrder)
            throw new AppException("HOME_SECTION_REORDER_INVALID",
                "La sección ya tiene ese orden.", 400);

        var other = await _repository.GetByDisplayOrderAsync(dto.NewDisplayOrder);

        if (other is null)
            throw new AppException("HOME_SECTION_REORDER_INVALID",
                $"No existe ninguna sección con el orden {dto.NewDisplayOrder}.", 400);

        var originalOrder = section.DisplayOrder;

        other.SetDisplayOrder(originalOrder);
        section.SetDisplayOrder(dto.NewDisplayOrder);

        await _repository.SaveChangesAsync();
    }
}