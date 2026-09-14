using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;

namespace PaginaVentasNet.Api.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    protected ActionResult<ApiResponse<T>> Success<T>(T data, int statusCode = 200)
    {
        return StatusCode(statusCode, ApiResponse<T>.Ok(data, statusCode));
    }

    protected ActionResult<ApiResponse<T>> Failure<T>(string code, string message, int statusCode = 400)
    {
        return StatusCode(statusCode, ApiResponse<T>.Fail(code, message, statusCode));
    }
}