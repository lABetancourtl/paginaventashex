using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;

namespace PaginaVentasNet.Api.Controllers;

/// <summary>
/// Clase base para los controladores de la API.
/// </summary>
[ApiController]
public class ApiController : ControllerBase
{

    /// <summary>
    /// Retorna una respuesta exitosa.
    /// </summary>
    /// <typeparam name="T">Tipo de dato de la respuesta.</typeparam>
    /// <param name="data">Datos de la respuesta.</param>
    /// <param name="statusCode">Código de estado HTTP.</param>
    /// <returns>Respuesta exitosa con los datos proporcionados.</returns>
    protected ActionResult<ApiResponse<T>> Success<T>(T data, int statusCode = 200)
    {
        return StatusCode(statusCode, ApiResponse<T>.Ok(data, statusCode));
    }

    /// <summary>
    /// Retorna una respuesta de error.
    /// </summary>
    /// <typeparam name="T">Tipo de dato de la respuesta.</typeparam>
    /// <param name="code">Código de error.</param>
    /// <param name="message">Mensaje de error.</param>
    /// <param name="statusCode">Código de estado HTTP.</param>
    /// <returns>Respuesta de error con el código y mensaje proporcionados.</returns>
    protected ActionResult<ApiResponse<T>> Failure<T>(string code, string message, int statusCode = 400)
    {
        return StatusCode(statusCode, ApiResponse<T>.Fail(code, message, statusCode));
    }
}