using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;
using PaginaVentasNet.Api.Modules.Home.Domain;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Activa o desactiva una sección. Una sección desactivada no sale en la página de inicio,
/// pero conserva su configuración.
/// </summary>
public class ChangeHomeSectionStatusUseCase
{
    private readonly IHomeSectionRepository _repository;

    public ChangeHomeSectionStatusUseCase(IHomeSectionRepository repository)
    {
        _repository = repository;
    }

    public async Task ActivateAsync(int id)
    {
        var section = await GetOrThrowAsync(id);

        if (section.IsActive)
            throw new AppException("HOME_SECTION_INVALID_STATE", "La sección ya está activa.", 400);

        section.Activate();
        await _repository.SaveChangesAsync();
    }

    public async Task DeactivateAsync(int id)
    {
        var section = await GetOrThrowAsync(id);

        if (!section.IsActive)
            throw new AppException("HOME_SECTION_INVALID_STATE", "La sección ya está desactivada.", 400);

        section.Deactivate();
        await _repository.SaveChangesAsync();
    }

    private async Task<HomeSection> GetOrThrowAsync(int id)
    {
        var section = await _repository.GetEntityByIdAsync(id);

        if (section is null)
            throw new AppException("HOME_SECTION_NOT_FOUND",
                $"No se encontró la sección con Id {id}.", 404);

        return section;
    }
}