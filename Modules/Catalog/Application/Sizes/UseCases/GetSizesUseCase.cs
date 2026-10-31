using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Sizes.UseCases;

/// <summary>
/// Lista las tallas, opcionalmente de un solo grupo. Por defecto solo las activas.
/// </summary>
public class GetSizesUseCase
{
    private readonly ISizeRepository _repository;

    public GetSizesUseCase(ISizeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<SizeResponseDto>> ExecuteAsync(SizeGroup? sizeGroup, bool includeInactive)
    {
        return await _repository.GetAllAsync(sizeGroup, includeInactive);
    }
}