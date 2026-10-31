using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Colors.UseCases;

/// <summary>
/// Activa o desactiva un color. Un color desactivado deja de ofrecerse en el desplegable,
/// pero las variantes que ya lo usan siguen funcionando.
/// </summary>
public class ChangeColorStatusUseCase
{
    private readonly IColorRepository _repository;

    public ChangeColorStatusUseCase(IColorRepository repository)
    {
        _repository = repository;
    }

    public async Task ActivateAsync(int id)
    {
        var color = await GetOrThrowAsync(id);

        if (color.IsActive)
            throw new AppException("COLOR_INVALID_STATE", "El color ya está activo.", 400);

        color.Activate();
        await _repository.SaveChangesAsync();
    }

    public async Task DeactivateAsync(int id)
    {
        var color = await GetOrThrowAsync(id);

        if (!color.IsActive)
            throw new AppException("COLOR_INVALID_STATE", "El color ya está desactivado.", 400);

        color.Deactivate();
        await _repository.SaveChangesAsync();
    }

    private async Task<Color> GetOrThrowAsync(int id)
    {
        var color = await _repository.GetEntityByIdAsync(id);

        if (color is null)
            throw new AppException("COLOR_NOT_FOUND",
                $"No se encontró el color con Id {id}.", 404);

        return color;
    }
}