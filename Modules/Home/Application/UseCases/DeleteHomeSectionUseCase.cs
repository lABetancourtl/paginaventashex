using Microsoft.Extensions.Logging;
using PaginaVentasNet.Api.Common.Exceptions;
using PaginaVentasNet.Api.Modules.Catalog.Application.Media.Ports;
using PaginaVentasNet.Api.Modules.Home.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Home.Application.UseCases;

/// <summary>
/// Borra una sección con sus medios y sus productos (la base los elimina en cascada) y
/// los archivos de sus medios en Cloudinary. Las secciones siguientes suben un puesto.
/// </summary>
public class DeleteHomeSectionUseCase
{
    private readonly IHomeSectionRepository _repository;
    private readonly IHomeSectionItemRepository _items;
    private readonly ICloudinaryService _cloudinary;
    private readonly ILogger<DeleteHomeSectionUseCase> _logger;

    public DeleteHomeSectionUseCase(
        IHomeSectionRepository repository,
        IHomeSectionItemRepository items,
        ICloudinaryService cloudinary,
        ILogger<DeleteHomeSectionUseCase> logger)
    {
        _repository = repository;
        _items = items;
        _cloudinary = cloudinary;
        _logger = logger;
    }

    public async Task ExecuteAsync(int id)
    {
        var section = await _repository.GetEntityByIdAsync(id);

        if (section is null)
            throw new AppException("HOME_SECTION_NOT_FOUND",
                $"No se encontró la sección con Id {id}.", 404);

        var files = (await _items.GetBySectionIdAsync(id))
            .Select(i => (i.PublicId, i.MediaType))
            .ToList();

        var deletedOrder = section.DisplayOrder;
        var following = await _repository.GetWithOrderGreaterThanAsync(deletedOrder);

        await _repository.RemoveAsync(section);

        foreach (var other in following)
            other.SetDisplayOrder(other.DisplayOrder - 1);

        await _repository.SaveChangesAsync();

        foreach (var (publicId, mediaType) in files)
        {
            try
            {
                await _cloudinary.DeleteAsync(publicId, mediaType);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Se borró la sección {SectionId}, pero no se pudo borrar {PublicId} de Cloudinary.",
                    id, publicId);
            }
        }
    }
}