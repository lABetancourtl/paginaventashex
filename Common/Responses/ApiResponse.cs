namespace PaginaVentasNet.Api.Common.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }

    private ApiResponse() { }

    public static ApiResponse<T> Ok(T data, int statusCode = 200) => new()
    {
        Success = true,
        StatusCode = statusCode,
        Data = data,
        Error = null
    };

    public static ApiResponse<T> Fail(string code, string message, int statusCode = 400) => new()
    {
        Success = false,
        StatusCode = statusCode,
        Data = default,
        Error = new ApiError { Code = code, Message = message }
    };
}