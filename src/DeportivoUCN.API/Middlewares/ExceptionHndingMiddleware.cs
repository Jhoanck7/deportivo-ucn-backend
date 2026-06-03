using System.Security;
using System.Text.Json;
using DeportivoUCN.Application.DTO.BaseResponse;

namespace DeportivoUCN.API.Middlewares
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var traceId = Guid.NewGuid().ToString();
                context.Response.Headers["trace-id"] = traceId;

                var (statusCode, title) = MapExceptionToStatus(ex);

                ErrorDetail error = new ErrorDetail(title, ex.Message);

                _logger.LogError(ex, "Unhandled exception. Trace ID: {TraceId}", traceId);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                var json = JsonSerializer.Serialize(
                    error,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                );

                await context.Response.WriteAsync(json);
            }
        }

        private static (int, string) MapExceptionToStatus(Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException _ => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                ArgumentNullException _ => (StatusCodes.Status400BadRequest, "Bad request"),
                KeyNotFoundException _ => (StatusCodes.Status404NotFound, "Resource not found"),
                InvalidOperationException _ => (StatusCodes.Status409Conflict, "Operation conflict"),
                FormatException _ => (StatusCodes.Status400BadRequest, "Invalid format"),
                SecurityException _ => (StatusCodes.Status403Forbidden, "Forbidden access"),
                ArgumentOutOfRangeException _ => (StatusCodes.Status400BadRequest, "Argument out of range"),
                ArgumentException _ => (StatusCodes.Status400BadRequest, "Invalid argument"),
                TimeoutException _ => (StatusCodes.Status429TooManyRequests, "Too many requests"),
                JsonException _ => (StatusCodes.Status400BadRequest, "Invalid JSON"),
                _ => (StatusCodes.Status500InternalServerError, "Internal server error"),
            };
        }
    }
}