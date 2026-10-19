using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Dtos;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Application.Products.Ports;
using PaginaVentasNet.Api.Modules.Catalog.Domain.Enums;

namespace PaginaVentasNet.Api.Modules.Catalog.Application.Media.UseCases;

/// <summary>
/// Caso de uso para cambiar el rol de visualización de un archivo multimedia.
/// CatalogMain y CatalogHover son únicos por producto — si se asignan a una nueva foto,
/// la foto anterior pierde ese rol y queda como None.
/// </summary>
public class SetDisplayRoleUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IProductMediaRepository _mediaRepository;

    public SetDisplayRoleUseCase(
        IProductRepository productRepository,
        IProductMediaRepository mediaRepository)
    {
        _productRepository = productRepository;
        _mediaRepository = mediaRepository;
    }

    public async Task<bool> ExecuteAsync(int productId, int mediaId, SetDisplayRoleDto dto)
    {
        var product = await _productRepository.GetEntityByIdAsync(productId);

        if (product is null)
            throw new InvalidOperationException(
                $"No existe un producto con Id '{productId}'.");

        var media = await _mediaRepository.GetByIdAsync(mediaId);

        if (media is null || media.ProductId != productId)
            throw new InvalidOperationException(
                $"No existe un archivo multimedia con Id '{mediaId}' para este producto.");

        if (media.MediaType != MediaType.Image && dto.Role != DisplayRole.None)
            throw new ArgumentException(
                "Solo las imágenes pueden tener rol CatalogMain o CatalogHover.");

        if (media.DisplayRole == dto.Role)
            throw new ArgumentException(
                $"La imagen ya tiene el rol '{dto.Role}'.");

        // Si el rol es CatalogMain o CatalogHover, quita ese rol a quien lo tenga
        if (dto.Role == DisplayRole.CatalogMain || dto.Role == DisplayRole.CatalogHover)
        {
            var current = await _mediaRepository.GetByRoleAsync(productId, dto.Role);
            if (current is not null)
                current.SetDisplayRole(DisplayRole.None);
        }

        media.SetDisplayRole(dto.Role);
        await _mediaRepository.SaveChangesAsync();

        return true;
    }
}