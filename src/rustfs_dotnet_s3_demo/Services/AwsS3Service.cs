using Amazon.S3;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using rustfs.dotnet.s3.demo.Settings;

namespace rustfs.dotnet.s3.demo.Services;

public partial class AwsS3Service : IAwsS3Service
{
    private readonly IAmazonS3 _s3client;
    private readonly AwsS3Setting _settings;
    private readonly ILogger<AwsS3Service> _logger;

    public AwsS3Service(ILogger<AwsS3Service> logger, IOptionsSnapshot<AwsS3Setting> settings)
    {
        _logger = logger;
        _settings = settings.Value;

        var config = new AmazonS3Config
        {
            ServiceURL = _settings.ServiceUrl,
            ForcePathStyle = true,
            UseHttp = true
        };

        _s3client = new AmazonS3Client(
           credentials: new BasicAWSCredentials(_settings.AccessKey, _settings.SecretKey),
           clientConfig: config);
    }
}
