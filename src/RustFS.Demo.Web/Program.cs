using RustFS.Demo.Web.Infrastructure;
using RustFS.Demo.Web.Services;
using RustFS.Demo.Web.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 添加服务默认配置 (Aspire)
builder.AddServiceDefaults();

// 配置 S3StorageOptions
builder.Services.AddOptions<S3StorageOptions>()
    .Bind(builder.Configuration.GetSection(S3StorageOptions.SectionName))
    .Validate(options => options.IsValid(), $"{S3StorageOptions.SectionName} configuration is invalid");

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

    if (!string.IsNullOrWhiteSpace(options.AccessKey))
    {
        return new AmazonS3Client(
            new Amazon.Runtime.BasicAWSCredentials(
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

