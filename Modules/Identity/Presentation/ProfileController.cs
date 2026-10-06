using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Identity.Application.Profile.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Profile.UseCases;

namespace PaginaVentasNet.Api.Modules.Identity.Presentation;

/// <summary>
/// Controller para la gestión del perfil del usuario autenticado.
/// </summary>
[Authorize]
[Route("api/profile")]
public class ProfileController : ApiController
{
    private readonly GetProfileUseCase _getProfileUseCase;
    private readonly UpdateProfileUseCase _updateProfileUseCase;
    private readonly UpdatePasswordUseCase _updatePasswordUseCase;

    public ProfileController(
        GetProfileUseCase getProfileUseCase,
        UpdateProfileUseCase updateProfileUseCase,
        UpdatePasswordUseCase updatePasswordUseCase)
    {
        _getProfileUseCase = getProfileUseCase;
        _updateProfileUseCase = updateProfileUseCase;
        _updatePasswordUseCase = updatePasswordUseCase;
    }

    /// <summary>
    /// Obtiene el perfil del usuario autenticado.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ProfileResponseDto>>> GetProfile()
    {
        try
        {
            var usuarioId = GetUsuarioId();
            var profile = await _getProfileUseCase.ExecuteAsync(usuarioId);
            return Success(profile);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<ProfileResponseDto>("PROFILE_NOT_FOUND", ex.Message, 404);
        }
    }

    /// <summary>
    /// Actualiza los datos del perfil del usuario autenticado.
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateProfile(UpdateProfileDto dto)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            await _updateProfileUseCase.ExecuteAsync(usuarioId, dto);
            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("PROFILE_NOT_FOUND", ex.Message, 404);
        }
    }

    /// <summary>
    /// Asigna o cambia la contraseña del usuario autenticado.
    /// </summary>
    [HttpPut("password")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdatePassword(UpdatePasswordDto dto)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            await _updatePasswordUseCase.ExecuteAsync(usuarioId, dto);
            return Success(true);
        }
        catch (ArgumentException ex)
        {
            return Failure<bool>("PASSWORD_VALIDATION", ex.Message, 400);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("PROFILE_NOT_FOUND", ex.Message, 404);
        }
    }

    // [HttpPatch("deactivate")]
    // public async Task<ActionResult<ApiResponse<bool>>> DeactivateProfile()
    // {
    //     try
    //     {
    //         var usuarioId = GetUsuarioId();
    //         await _updateProfileUseCase.DeactivateProfileAsync(usuarioId);
    //         return Success(true);
    //     }
    //     catch (InvalidOperationException ex)
    //     {
    //         return Failure<bool>("PROFILE_NOT_FOUND", ex.Message, 404);
    //     }
    // }

    /// <summary>
    /// Obtiene el Id del usuario autenticado a partir de los claims del token. Lanza una excepción si no se puede obtener el Id.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private int GetUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst("sub");

        if (claim is null)
            throw new InvalidOperationException("No se pudo obtener el Id del usuario del token.");

        return int.Parse(claim.Value);
    }
}