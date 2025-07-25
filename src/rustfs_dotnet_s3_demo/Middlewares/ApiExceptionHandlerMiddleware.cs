using rustfs.dotnet.s3.demo.Handlers;

namespace rustfs.dotnet.s3.demo.Middlewares;

public class ApiExceptionHandlerMiddleware(
    ILogger<ApiExceptionHandlerMiddleware> logger,
    RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IExceptionHandler handler)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await handler.HandleExceptionAsync(context, ex);
        }
    }
}
