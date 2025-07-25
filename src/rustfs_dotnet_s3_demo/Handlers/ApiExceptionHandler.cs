using Amazon.S3;
using rustfs.dotnet.s3.demo.Models;
using System.Diagnostics;
using System.Net.Mime;

namespace rustfs.dotnet.s3.demo.Handlers;

public sealed class ApiExceptionHandler : IExceptionHandler
{
    public async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = CreateProblemDetailsFromException(context, exception);
        var apiResp = ApiResponse.Fail(problemDetails, "server error", problemDetails.Status);

        context.Response.ContentType = $"{MediaTypeNames.Application.ProblemJson};charset=utf-8";
        context.Response.StatusCode = problemDetails.Status;

        await context.Response.WriteAsJsonAsync(apiResp);
    }

    private static ProblemDetailsResponse CreateProblemDetailsFromException(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
        var dict = new Dictionary<string, object?> { ["traceId"] = traceId };

        var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
        bool isDev = env.IsDevelopment() || env.IsEnvironment("Dev");
        if (isDev)
        {
            dict["exp_stack_trace"] = exception.StackTrace;
        }

        return exception switch
        {
            BadHttpRequestException badRequest => ApiResponse.CreateProblemDetails(
                title: "Bad Request",
                status: 400,
                detail: isDev ? exception.Message : "The request is invalid",
                instance: context.Request.Path,
                extensions: dict
            ),
            UnauthorizedAccessException => ApiResponse.CreateProblemDetails(
                title: "Unauthorized",
                status: 401,
                detail: isDev ? exception.Message : "Access is denied",
                instance: context.Request.Path,
                extensions: dict
            ),
            InvalidOperationException => ApiResponse.CreateProblemDetails(
                title: "Invalid Operation",
                status: 400,
                detail: isDev ? exception.Message : "The operation is invalid",
                instance: context.Request.Path,
                extensions: dict
            ),
            HttpRequestException => ApiResponse.CreateProblemDetails(
                title: "Invalid http request",
                status: 500,
                detail: isDev ? exception.Message : "The http request is invalid",
                instance: context.Request.Path,
                extensions: dict
            ),
            AmazonS3Exception => ApiResponse.CreateProblemDetails(
                title: "Amazon S3 service exception",
                status: 500,
                detail: isDev ? exception.Message : "The amazon s3 service is invalid",
                instance: context.Request.Path,
                extensions: dict
            ),
            _ => ApiResponse.CreateProblemDetails(
                title: "Internal Server Error",
                status: 500,
                detail: isDev ? exception.Message : "An internal server error occurred",
                instance: context.Request.Path,
                extensions: dict
            )
        };
    }
}
