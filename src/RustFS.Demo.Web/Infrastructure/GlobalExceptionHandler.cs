using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RustFS.Demo.Web.Infrastructure;

/// <summary>
/// 全局异常处理器
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var (statusCode, title) = exception switch
        {
            ArgumentException or ArgumentNullException => (StatusCodes.Status400BadRequest, "Bad Request"),
            KeyNotFoundException or FileNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            InvalidOperationException => (StatusCodes.Status500InternalServerError, "Internal Server Error"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
