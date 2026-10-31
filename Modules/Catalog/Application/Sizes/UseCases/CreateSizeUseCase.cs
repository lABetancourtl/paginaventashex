using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.UseCases;

/// <summary>
/// Crea una talla nueva al final de su grupo.
/// </summary>
public class CreateSizeUseCase
{
    private readonly ISizeRepository _repository;

    public CreateSizeUseCase(ISizeRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> ExecuteAsync(CreateSizeDto dto)
    {
        var group = dto.SizeGroup!.Value;

        if (!Enum.IsDefined(group))
            throw new AppException("SIZE_VALIDATION",
                "El grupo de la talla no es válido.", 400);

        if (await _repository.ExistsByNameAsync(group, dto.Name))
            throw new AppException("SIZE_CONFLICT",
                $"Ya existe la talla '{dto.Name.Trim()}' en el grupo {group}.", 409);

        var displayOrder = await _repository.GetNextDisplayOrderAsync(group);
        var size = Size.Create(group, dto.Name, displayOrder);

        await _repository.AddAsync(size);
        await _repository.SaveChangesAsync();

        return size.Id;
    }
}