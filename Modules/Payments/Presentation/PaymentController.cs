using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Payments.Application.Dtos;
using PaginaVentasNet.Api.Modules.Payments.Application.UseCases;

namespace PaginaVentasNet.Api.Modules.Payments.Presentation;

/// <summary>
/// Controller para la integración con Wompi.
/// </summary>
[Route("api/payments")]
public class PaymentController : ApiController
{
    private readonly ProcessWebhookUseCase _processWebhookUseCase;
    private readonly GetPaymentInfoUseCase _getPaymentInfoUseCase;
    private readonly GetPaymentTransactionsUseCase _getTransactionsUseCase;

    public PaymentController(
        ProcessWebhookUseCase processWebhookUseCase,
        GetPaymentInfoUseCase getPaymentInfoUseCase,
        GetPaymentTransactionsUseCase getTransactionsUseCase)
    {
        _processWebhookUseCase = processWebhookUseCase;
        _getPaymentInfoUseCase = getPaymentInfoUseCase;
        _getTransactionsUseCase = getTransactionsUseCase;
    }

    /// <summary>
    /// Webhook que Wompi llama cuando una transacción cambia de estado.
    /// No requiere autenticación JWT — Wompi lo llama directamente.
    /// </summary>
    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook([FromBody] WompiWebhookDto dto)
    {
        try
        {
            var rawPayload = System.Text.Json.JsonSerializer.Serialize(dto);
            await _processWebhookUseCase.ExecuteAsync(dto, rawPayload);
            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    /// <summary>
    /// Obtiene los datos necesarios para iniciar el pago en Wompi desde el frontend.
    /// </summary>
    [HttpGet("orders/{orderId}/payment-info")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PaymentInfoDto>>> GetPaymentInfo(int orderId)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            var info = await _getPaymentInfoUseCase.ExecuteAsync(orderId, usuarioId);
            return Success(info);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<PaymentInfoDto>("PAYMENT_ERROR", ex.Message, 400);
        }
    }

    /// <summary>
    /// Obtiene el historial completo de transacciones de pago. Solo Admin.
    /// </summary>
    [HttpGet("transactions")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<List<PaymentTransactionResponseDto>>>> GetTransactions()
    {
        var transactions = await _getTransactionsUseCase.ExecuteAsync();
        return Success(transactions);
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