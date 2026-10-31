using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Colors.UseCases;

/// <summary>
/// Edita el nombre y el código de un color. Reemplaza el color completo:
/// si no se envía hexCode, el color queda sin muestra.
/// </summary>
public class UpdateColorUseCase
{
    private readonly IColorRepository _repository;

    public UpdateColorUseCase(IColorRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(int id, UpdateColorDto dto)
    {
        var color = await _repository.GetEntityByIdAsync(id);

        if (color is null)
            throw new AppException("COLOR_NOT_FOUND",
                $"No se encontró el color con Id {id}.", 404);

        if (await _repository.ExistsByNameAsync(dto.Name, excludeId: id))
            throw new AppException("COLOR_CONFLICT",
                $"Ya existe el color '{dto.Name.Trim()}'.", 409);

        try
        {
            color.Update(dto.Name, dto.HexCode);
        }
        catch (ArgumentException ex)
        {
            throw new AppException("COLOR_VALIDATION", ex.Message, 400);
        }

        await _repository.SaveChangesAsync();
    }
}