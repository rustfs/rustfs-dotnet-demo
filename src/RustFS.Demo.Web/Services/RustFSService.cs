using Amazon.S3;
using Microsoft.Extensions.Options;
using RustFS.Demo.Web.Models;

namespace RustFS.Demo.Web.Services;

/// <summary>
/// RustFS 服务实现类，基于 AWS SDK for .NET
/// </summary>
public partial class RustFSService : IRustFSService
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<RustFSService> _logger;
    private readonly string _serviceUrl;

    public RustFSService(ILogger<RustFSService> logger, IAmazonS3 s3Client, IOptions<S3StorageOptions> options)
    {
        _logger = logger;
        _s3Client = s3Client;
        _serviceUrl = options.Value.Endpoint?.AbsoluteUri ?? "http://localhost:9000";
    }
}
