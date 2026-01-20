using RustFS.Demo.Web.Infrastructure;
using RustFS.Demo.Web.Services;
using Scalar.AspNetCore;
using Amazon.S3;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using RustFS.Demo.Web.Models;
using Amazon;

var builder = WebApplication.CreateBuilder(args);

// 添加服务默认配置 (Aspire)
builder.AddServiceDefaults();

// 配置 S3StorageOptions
builder.Services.Configure<S3StorageOptions>(options =>
{
    var config = builder.Configuration;
    var serviceUrl = config["services:rustfs:s3-api:0"] ?? config["RustFS:ServiceUrl"] ?? "http://localhost:9000";
    options.Endpoint = new Uri(serviceUrl);
    options.AccessKey = config["RustFS:AccessKey"] ?? "admin";
    options.SecretKey = config["RustFS:SecretKey"] ?? "admin123";
    options.Region = "us-east-1";
});

// 注册 AmazonS3Client
builder.Services.AddSingleton<IAmazonS3>(provider =>
{
    var options = provider.GetRequiredService<IOptions<S3StorageOptions>>().Value;

    var config = options.Endpoint != null
        ? new AmazonS3Config
        {
            ServiceURL = options.Endpoint.AbsoluteUri,
            ForcePathStyle = true, // RustFS/MinIO 需要
            UseHttp = true
        }
        : new AmazonS3Config { RegionEndpoint = RegionEndpoint.GetBySystemName(options.Region) };

    if (options.UseInstanceProfile)
    {
        return new AmazonS3Client(config);
    }

    if (!string.IsNullOrEmpty(options.AccessKey))
    {
        return new AmazonS3Client(
            new BasicAWSCredentials(
                options.AccessKey,
                options.SecretKey),
            config);
    }

    return new AmazonS3Client(config);
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 注册异常处理
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 注册 RustFSService
builder.Services.AddSingleton<IRustFSService, RustFSService>();

var app = builder.Build();

// 配置 HTTP 请求管道
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

// 启用静态文件 (wwwroot)
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

await app.RunAsync();

