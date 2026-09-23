using Microsoft.AspNetCore.Diagnostics;
using PaginaVentasNet.Api.Common.Responses;

namespace PaginaVentasNet.Api.Common.Middleware;

/// <summary>
/// Middleware para manejar excepciones globales.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Maneja las excepciones no controladas y devuelve una respuesta de error genérica.
    /// </summary>
    /// <param name="httpContext">Contexto HTTP actual.</param>
    /// <param name="exception">Excepción no controlada.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>True si la excepción fue manejada, false en caso contrario.</returns
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Excepción no controlada: {Message}", exception.Message);

        var response = ApiResponse<object>.Fail(
            "INTERNAL_SERVER_ERROR",
            "Ocurrió un error inesperado. Por favor intente más tarde.",
            500);

        httpContext.Response.StatusCode = 500;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}