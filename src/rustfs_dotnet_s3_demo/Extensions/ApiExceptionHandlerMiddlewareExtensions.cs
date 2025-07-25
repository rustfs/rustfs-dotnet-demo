using rustfs.dotnet.s3.demo.Middlewares;

namespace rustfs.dotnet.s3.demo.Extensions;

/// <summary>
/// 中间件扩展方法
/// </summary>
internal static class ApiExceptionHandlerMiddlewareExtensions
{
    /// <summary>
    /// 添加API异常处理中间件
    /// </summary>
    /// <param name="app">应用程序构建器</param>
    /// <returns>应用程序构建器</returns>
    public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiExceptionHandlerMiddleware>();
    }
}
