using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Identity.Application.Users.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Users.UseCases;

namespace PaginaVentasNet.Api.Modules.Identity.Presentation;

/// <summary>
/// Controller para la gestión de usuarios.
/// Solo accesible por administradores.
/// </summary>
[Authorize(Roles = "Admin")]
[Route("api/users")]
public class UserController : ApiController
{
    private readonly ChangeRolUseCase _changeRolUseCase;

    public UserController(ChangeRolUseCase changeRolUseCase)
    {
        _changeRolUseCase = changeRolUseCase;
    }

    /// <summary>
    /// Cambia el rol de un usuario.
    /// </summary>
    /// <param name="id">Id del usuario.</param>
    /// <param name="dto">Nuevo rol: Admin o Cliente.</param>
    [HttpPatch("{id}/role")]
    public async Task<ActionResult<ApiResponse<bool>>> ChangeRol(int id, ChangeRolDto dto)
    {
        try
        {
            await _changeRolUseCase.ExecuteAsync(id, dto);
            return Success(true);
        }
        catch (ArgumentException ex)
        {
            return Failure<bool>("ROL_VALIDATION", ex.Message, 400);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("USER_NOT_FOUND", ex.Message, 404);
        }
    }
}