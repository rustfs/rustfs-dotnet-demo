using rustfs.dotnet.s3.demo.Extensions;
using rustfs.dotnet.s3.demo.Handlers;
using rustfs.dotnet.s3.demo.Services;
using rustfs.dotnet.s3.demo.Settings;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Aws S3
builder.Services.Configure<AwsS3Setting>(builder.Configuration.GetSection(AwsS3Setting.Section));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// add api exception handler
builder.Services.AddSingleton<IExceptionHandler, ApiExceptionHandler>();

// add S3 service
builder.Services.AddScoped<IAwsS3Service, AwsS3Service>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // scalar/v1
}

// 处理 404 等状态码
app.UseStatusCodePages(); 

// 使用自定义异常处理(ProblemDetails)中间件
app.UseApiExceptionHandler();

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
