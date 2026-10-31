using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.UseCases;

/// <summary>
/// Renombra una talla. El grupo no se cambia, porque rompería la numeración de orden.
/// </summary>
public class UpdateSizeUseCase
{
    private readonly ISizeRepository _repository;

    public UpdateSizeUseCase(ISizeRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(int id, UpdateSizeDto dto)
    {
        var size = await _repository.GetEntityByIdAsync(id);

        if (size is null)
            throw new AppException("SIZE_NOT_FOUND",
                $"No se encontró la talla con Id {id}.", 404);

        if (await _repository.ExistsByNameAsync(size.SizeGroup, dto.Name, excludeId: id))
            throw new AppException("SIZE_CONFLICT",
                $"Ya existe la talla '{dto.Name.Trim()}' en el grupo {size.SizeGroup}.", 409);

        size.Rename(dto.Name);
        await _repository.SaveChangesAsync();
    }
}