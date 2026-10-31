using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Colors.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Colors.UseCases;

/// <summary>
/// Crea un color nuevo. El nombre no se repite sin importar mayúsculas.
/// </summary>
public class CreateColorUseCase
{
    private readonly IColorRepository _repository;

    public CreateColorUseCase(IColorRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> ExecuteAsync(CreateColorDto dto)
    {
        if (await _repository.ExistsByNameAsync(dto.Name))
            throw new AppException("COLOR_CONFLICT",
                $"Ya existe el color '{dto.Name.Trim()}'.", 409);

        Color color;

        try
        {
            color = Color.Create(dto.Name, dto.HexCode);
        }
        catch (ArgumentException ex)
        {
            throw new AppException("COLOR_VALIDATION", ex.Message, 400);
        }

        await _repository.AddAsync(color);
        await _repository.SaveChangesAsync();

        return color.Id;
    }
}