using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Identity.Application.Auth.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Otp.Dtos;
using PaginaVentasNet.Api.Modules.Identity.Application.Otp.UseCases;

namespace PaginaVentasNet.Api.Modules.Identity.Presentation;

/// <summary>
/// Controller para autenticación sin contraseña mediante código OTP.
/// </summary>
[Route("api/auth/otp")]
public class OtpController : ApiController
{
    private readonly SendOtpUseCase _sendOtpUseCase;
    private readonly VerifyOtpUseCase _verifyOtpUseCase;

    public OtpController(SendOtpUseCase sendOtpUseCase, VerifyOtpUseCase verifyOtpUseCase)
    {
        _sendOtpUseCase = sendOtpUseCase;
        _verifyOtpUseCase = verifyOtpUseCase;
    }

    /// <summary>
    /// Envía un código OTP al email indicado.
    /// Funciona tanto para usuarios nuevos como existentes.
    /// </summary>
    /// <param name="dto">Email del usuario.</param>
    [HttpPost("send")]
    [EnableRateLimiting("otp")]
    public async Task<ActionResult<ApiResponse<bool>>> Send(SendOtpDto dto)
    {
        try
        {
            await _sendOtpUseCase.ExecuteAsync(dto);
            return Success(true);
        }
        catch (ArgumentException ex)
        {
            return Failure<bool>("OTP_VALIDATION", ex.Message, 400);
        }
    }

    /// <summary>
    /// Verifica el código OTP e inicia sesión.
    /// Si el usuario no existe lo crea automáticamente.
    /// </summary>
    /// <param name="dto">Email y código OTP.</param>
    /// <returns>Token JWT válido.</returns>
    [HttpPost("verify")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Verify(VerifyOtpDto dto)
    {
        try
        {
            var response = await _verifyOtpUseCase.ExecuteAsync(dto);
            return Success(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Failure<AuthResponseDto>("OTP_INVALID", ex.Message, 401);
        }
    }
}