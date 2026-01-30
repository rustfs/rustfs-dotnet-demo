using RustFS.Demo.Web.Options;

namespace RustFS.Demo.Web.Services;

/// <summary>
/// RustFS 服务实现类，基于 AWS SDK for .NET
/// </summary>
public partial class RustFSService(
    ILogger<RustFSService> logger, 
    IAmazonS3 s3Client, 
    IOptions<S3StorageOptions> options) : IRustFSService
{
    private readonly string _serviceUrl = options.Value.Endpoint?.AbsoluteUri ?? "http://localhost:9000";
}
