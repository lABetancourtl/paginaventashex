using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Borra una sección con sus medios y sus productos (la base los elimina en cascada).
/// Las secciones siguientes suben un puesto, para que el orden siga consecutivo.
/// </summary>
public class DeleteHomeSectionUseCase
{
    private readonly IHomeSectionRepository _repository;

    public DeleteHomeSectionUseCase(IHomeSectionRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(int id)
    {
        var section = await _repository.GetEntityByIdAsync(id);

        if (section is null)
            throw new AppException("HOME_SECTION_NOT_FOUND",
                $"No se encontró la sección con Id {id}.", 404);

        var deletedOrder = section.DisplayOrder;
        var following = await _repository.GetWithOrderGreaterThanAsync(deletedOrder);

        await _repository.RemoveAsync(section);

        foreach (var item in following)
            item.SetDisplayOrder(item.DisplayOrder - 1);

        await _repository.SaveChangesAsync();
    }
}