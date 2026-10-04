using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Products.UseCases;

/// <summary>
/// Caso de uso para búsqueda de productos desde el panel de administración.
/// Permite filtrar por múltiples categorías, nombre y estado activo/inactivo.
/// </summary>
public class AdminSearchProductsUseCase
{
    private readonly IProductRepository _repository;

    public AdminSearchProductsUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<ProductResponseDto>> ExecuteAsync(AdminSearchProductsDto dto)
    {
        if (dto.Page < 1)
            dto.Page = 1;

        return await _repository.AdminSearchAsync(dto);
    }
}