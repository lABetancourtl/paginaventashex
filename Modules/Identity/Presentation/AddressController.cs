using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Addresses.UseCases;

namespace PaginaVentasNet.Api.Modules.Identity.Presentation;

/// <summary>
/// Controller para la gestión de direcciones del usuario autenticado.
/// </summary>
[Authorize]
[Route("api/profile/addresses")]
public class AddressController : ApiController
{
    private readonly CreateDireccionUseCase _createUseCase;
    private readonly GetDireccionesUseCase _getUseCase;
    private readonly DeleteDireccionUseCase _deleteUseCase;
    private readonly SetDireccionPrincipalUseCase _setPrincipalUseCase;

    public AddressController(
        CreateDireccionUseCase createUseCase,
        GetDireccionesUseCase getUseCase,
        DeleteDireccionUseCase deleteUseCase,
        SetDireccionPrincipalUseCase setPrincipalUseCase)
    {
        _createUseCase = createUseCase;
        _getUseCase = getUseCase;
        _deleteUseCase = deleteUseCase;
        _setPrincipalUseCase = setPrincipalUseCase;
    }

    /// <summary>
    /// Obtiene todas las direcciones del usuario autenticado.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<DireccionResponseDto>>>> GetAll()
    {
        var usuarioId = GetUsuarioId();
        var direcciones = await _getUseCase.ExecuteAsync(usuarioId);
        return Success(direcciones);
    }

    /// <summary>
    /// Agrega una nueva dirección al usuario autenticado.
    /// Si es la primera dirección se marca automáticamente como principal.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<int>>> Create(CreateDireccionDto dto)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            var id = await _createUseCase.ExecuteAsync(usuarioId, dto);
            return Success(id, 201);
        }
        catch (ArgumentException ex)
        {
            return Failure<int>("ADDRESS_VALIDATION", ex.Message, 400);
        }
    }

    /// <summary>
    /// Elimina una dirección del usuario autenticado.
    /// Si era la principal, asigna la siguiente como principal automáticamente.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            await _deleteUseCase.ExecuteAsync(usuarioId, id);
            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("ADDRESS_NOT_FOUND", ex.Message, 404);
        }
    }

    /// <summary>
    /// Establece una dirección como principal del usuario autenticado.
    /// </summary>
    [HttpPatch("{id}/principal")]
    public async Task<ActionResult<ApiResponse<bool>>> SetPrincipal(int id)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            await _setPrincipalUseCase.ExecuteAsync(usuarioId, id);
            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("ADDRESS_NOT_FOUND", ex.Message, 404);
        }
        catch (ArgumentException ex)
        {
            return Failure<bool>("ADDRESS_ALREADY_PRINCIPAL", ex.Message, 400);
        }
    }

    private int GetUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst("sub");

        if (claim is null)
            throw new InvalidOperationException("No se pudo obtener el Id del usuario del token.");

        return int.Parse(claim.Value);
    }
}