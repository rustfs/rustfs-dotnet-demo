using System.Net;
using System.Text;
using Amazon.S3;
using Amazon.S3.Transfer;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace RustFS.Demo.Tests;

/// <summary>
/// Integration test: Reproduce the issue where TransferUtility fails to upload when Checksum Validation is enabled by default.
/// https://github.com/rustfs/rustfs/issues/1731
/// </summary>
public class ChecksumTests : IAsyncLifetime
{
    private readonly IContainer _rustfsContainer;
    private const int S3Port = 9000;
    private const string AccessKey = "admin";
    private const string SecretKey = "admin123";

    public ChecksumTests()
    {
        _rustfsContainer = new ContainerBuilder(new DotNet.Testcontainers.Images.DockerImage("rustfs/rustfs:latest"))
            .WithPortBinding(S3Port, true)
            .WithEnvironment("RUSTFS_ADDRESS", "0.0.0.0:9000")
            .WithEnvironment("RUSTFS_ACCESS_KEY", AccessKey)
            .WithEnvironment("RUSTFS_SECRET_KEY", SecretKey)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r =>
                r.ForPort(S3Port).ForPath("/").ForStatusCode(HttpStatusCode.Forbidden)))
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _rustfsContainer.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _rustfsContainer.StopAsync();
        GC.SuppressFinalize(this);
    }

    private AmazonS3Client CreateS3Client()
    {
        var hostPort = _rustfsContainer.GetMappedPublicPort(S3Port);
        var serviceUrl = $"http://{_rustfsContainer.Hostname}:{hostPort}";

        var config = new AmazonS3Config
        {
            ServiceURL = serviceUrl,
            ForcePathStyle = true,
            UseHttp = true
        };

        return new AmazonS3Client(AccessKey, SecretKey, config);
    }

    [Fact]
    public async Task Upload_WithChecksumDisabled_ShouldSucceed()
    {
        // Arrange
        var s3Client = CreateS3Client();
        var bucketName = $"bucket-no-checksum-{Guid.NewGuid()}";
        var fileName = "test-file-no-checksum.txt";
        var content = "This is a test file uploaded without checksum validation.";

        await s3Client.PutBucketAsync(bucketName, TestContext.Current.CancellationToken);

        var transferUtility = new TransferUtility(s3Client);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var request = new TransferUtilityUploadRequest
        {
            BucketName = bucketName,
            Key = fileName,
            InputStream = stream,
            ContentType = "text/plain",
            // Explicitly disable Checksum validation, expect success
            DisableDefaultChecksumValidation = true
        };

        await transferUtility.UploadAsync(request, TestContext.Current.CancellationToken);

        // Assert
        // Verify file existence
        var response = await s3Client.GetObjectAsync(bucketName, fileName, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.HttpStatusCode);

        using var reader = new StreamReader(response.ResponseStream);
        var uploadedContent = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        Assert.Equal(content, uploadedContent);
    }

    /// <summary>
    /// Verifies the current RustFS behavior when using default checksum validation.
    /// Large files trigger multipart upload and should upload successfully without disabling checksum validation.
    /// </summary>
    [Fact]
    public async Task Upload_LargeFile_WithDefaultChecksum_ShouldSucceed()
    {
        // Arrange
        var s3Client = CreateS3Client();
        var bucketName = $"bucket-with-checksum-{Guid.NewGuid()}";
        var fileName = "test-file-with-checksum.txt";

        // Use a large file size (20MB) to trigger Multipart Upload
        // TransferUtility default threshold is usually around 16MB
        var fileSize = 20 * 1024 * 1024;
        var content = new byte[fileSize];
        new Random().NextBytes(content);

        await s3Client.PutBucketAsync(bucketName, TestContext.Current.CancellationToken);

        var transferUtility = new TransferUtility(s3Client);
        using var stream = new MemoryStream(content);

        // Act
        var request = new TransferUtilityUploadRequest
        {
            BucketName = bucketName,
            Key = fileName,
            InputStream = stream,
            ContentType = "application/octet-stream"
            // Default DisableDefaultChecksumValidation = false
        };

        await transferUtility.UploadAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var response = await s3Client.GetObjectAsync(bucketName, fileName, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.HttpStatusCode);
    }
}
