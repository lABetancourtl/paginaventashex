using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Auth.UseCases;

namespace PaginaVentasNet.Api.Modules.Identity.Presentation;

[Route("api/auth")]
public class AuthController : ApiController
{
    private readonly RegisterUseCase _registerUseCase;
    private readonly LoginUseCase _loginUseCase;

    public AuthController(RegisterUseCase registerUseCase, LoginUseCase loginUseCase)
    {
        _registerUseCase = registerUseCase;
        _loginUseCase = loginUseCase;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(RegisterDto dto)
    {
        try
        {
            var response = await _registerUseCase.ExecuteAsync(dto);
            return Success(response, 201);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<AuthResponseDto>("EMAIL_ALREADY_EXISTS", ex.Message, 409);
        }
    }

    [HttpPost("login")]
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