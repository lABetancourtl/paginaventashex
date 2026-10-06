using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Auth.UseCases;

namespace PaginaVentasNet.Api.Modules.Identity.Presentation;

[Route("api/auth")]
public class AuthController : ApiController
{

    private readonly LoginUseCase _loginUseCase;

    public AuthController(LoginUseCase loginUseCase)
    {
        _loginUseCase = loginUseCase;
    }


    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginDto dto)
    {
        try
        {
            var response = await _loginUseCase.ExecuteAsync(dto);
            return Success(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Failure<AuthResponseDto>("INVALID_CREDENTIALS", ex.Message, 401);
        }
    }
}