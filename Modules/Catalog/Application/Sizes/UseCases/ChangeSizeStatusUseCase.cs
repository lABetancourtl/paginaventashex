using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.UseCases;

/// <summary>
/// Activa o desactiva una talla. Una talla desactivada deja de ofrecerse en el desplegable,
/// pero las variantes que ya la usan siguen funcionando.
/// </summary>
public class ChangeSizeStatusUseCase
{
    private readonly ISizeRepository _repository;

    public ChangeSizeStatusUseCase(ISizeRepository repository)
    {
        _repository = repository;
    }

    public async Task ActivateAsync(int id)
    {
        var size = await GetOrThrowAsync(id);

        if (size.IsActive)
            throw new AppException("SIZE_INVALID_STATE", "La talla ya está activa.", 400);

        size.Activate();
        await _repository.SaveChangesAsync();
    }

    public async Task DeactivateAsync(int id)
    {
        var size = await GetOrThrowAsync(id);

        if (!size.IsActive)
            throw new AppException("SIZE_INVALID_STATE", "La talla ya está desactivada.", 400);

        size.Deactivate();
        await _repository.SaveChangesAsync();
    }

    private async Task<Domain.Size> GetOrThrowAsync(int id)
    {
        var size = await _repository.GetEntityByIdAsync(id);

        if (size is null)
            throw new AppException("SIZE_NOT_FOUND",
                $"No se encontró la talla con Id {id}.", 404);

        return size;
    }
}