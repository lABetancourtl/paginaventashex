namespace PaginaVentasNet.Api.Common.Responses;

/// <summary>
/// Clase que representa una respuesta de la API.
/// </summary>
/// <typeparam name="T">Tipo de dato de la respuesta.</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }

    private ApiResponse() { }

    /// <summary>
    /// Crea una respuesta exitosa.
    /// </summary>
    /// <param name="data">Datos de la respuesta.</param>
    /// <param name="statusCode">Código de estado HTTP.</param>
    /// <returns>Respuesta exitosa con los datos proporcionados.</returns>
    public static ApiResponse<T> Ok(T data, int statusCode = 200) => new()
    {
        Success = true,
        StatusCode = statusCode,
        Data = data,
        Error = null
    };

    /// <summary>
    /// Crea una respuesta fallida.
    /// </summary>
    /// <param name="code">Código de error.</param>
    /// <param name="message">Mensaje de error.</param>
    /// <param name="statusCode">Código de estado HTTP.</param>
    /// <returns>Respuesta fallida con el código y mensaje de error proporcionados.</returns
    public static ApiResponse<T> Fail(string code, string message, int statusCode = 400) => new()
    {
        Success = false,
        StatusCode = statusCode,
        Data = default,
        Error = new ApiError { Code = code, Message = message }
    };
}