namespace rustfs.dotnet.s3.demo.Handlers;

public interface IExceptionHandler
{
    Task HandleExceptionAsync(HttpContext context, Exception exception);
}
