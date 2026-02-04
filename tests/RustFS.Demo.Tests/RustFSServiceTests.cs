using System.Net;
using System.Text;
using Amazon.S3;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RustFS.Demo.Web.Models;
using RustFS.Demo.Web.Services;

namespace RustFS.Demo.Tests;

public class RustFSServiceTests : IAsyncLifetime
{
    private readonly IContainer _rustfsContainer;
    private const int S3Port = 9000;
    private const string AccessKey = "admin";
    private const string SecretKey = "admin123";

    public RustFSServiceTests()
    {
        _rustfsContainer = new ContainerBuilder(new DockerImage("rustfs/rustfs:latest"))
            .WithPortBinding(S3Port, true) // Map container port 9000 to a random host port
            .WithEnvironment("RUSTFS_ADDRESS", "0.0.0.0:9000")
            .WithEnvironment("RUSTFS_ACCESS_KEY", AccessKey)
            .WithEnvironment("RUSTFS_SECRET_KEY", SecretKey)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r =>
                r.ForPort(S3Port).ForPath("/").ForStatusCode(HttpStatusCode.Forbidden))) // Expect 403 for unauthenticated root access
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

    private RustFSService CreateService()
    {
        var hostPort = _rustfsContainer.GetMappedPublicPort(S3Port);
        var serviceUrl = $"http://{_rustfsContainer.Hostname}:{hostPort}";

        var options = new S3StorageOptions
        {
            Endpoint = new Uri(serviceUrl),
            AccessKey = AccessKey,
            SecretKey = SecretKey,
            Region = "us-east-1"
        };

        var s3Config = new AmazonS3Config
        {
            ServiceURL = options.Endpoint.AbsoluteUri,
            ForcePathStyle = true, // MinIO/RustFS usually requires this
            UseHttp = true
        };

        var s3Client = new AmazonS3Client(options.AccessKey, options.SecretKey, s3Config);
        var logger = NullLogger<RustFSService>.Instance;
        var optionsWrapper = Options.Create(options);

        return new RustFSService(logger, s3Client, optionsWrapper);
    }

    [Fact]
    public async Task PresignedUploadUrl_ShouldWorkCorrectly()
    {
        // Arrange
        var service = CreateService();
        var bucketName = $"bucket-{Guid.NewGuid()}";
        var fileName = "test-upload.txt";
        var content = "Hello, Presigned URL!";
        var contentType = "text/plain";

        // Act
        // 1. Generate Presigned URL (will create bucket if not exists)
        var options = new PresignedUrlOptions(fileName, contentType, bucketName);
        var url = await service.GeneratePresignedUploadUrlAsync(options);
        Assert.NotNull(url);
        Assert.StartsWith("http://", url);

        // 2. Upload file using the URL
        using var httpClient = new HttpClient();
        var contentBytes = Encoding.UTF8.GetBytes(content);
        using var byteContent = new ByteArrayContent(contentBytes);
        byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

        var response = await httpClient.PutAsync(url, byteContent, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.IsSuccessStatusCode, $"Upload failed with status code: {response.StatusCode}");

        // 3. Verify file exists
        var exists = await service.BucketExistsAsync(bucketName);
        Assert.True(exists, "Bucket should exist");

        var files = await service.ListFilesAsync(bucketName);
        Assert.Contains(fileName, files);

        // 4. Verify content
        await using var stream = await service.GetFileAsync(bucketName, fileName);
        using var reader = new StreamReader(stream);
        var uploadedContent = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        Assert.Equal(content, uploadedContent);
    }

    [Fact]
    public async Task BucketOperations_ShouldWorkCorrectly()
    {
        // Arrange
        var service = CreateService();
        var bucketName = $"bucket-{Guid.NewGuid()}";

        // Act & Assert
        // 1. Create Bucket
        var createResult = await service.CreateBucketAsync(bucketName);
        Assert.True(createResult, "CreateBucketAsync failed");

        // 2. Check Exists
        var exists = await service.BucketExistsAsync(bucketName);
        Assert.True(exists, "BucketExistsAsync returned false after creation");

        // 3. List Buckets
        var buckets = await service.ListBucketsAsync();
        Assert.Contains(bucketName, buckets);

        // 4. Delete Bucket
        var deleteResult = await service.DeleteBucketAsync(bucketName);
        Assert.True(deleteResult, "DeleteBucketAsync failed");

        // 5. Verify Deletion
        var existsAfterDelete = await service.BucketExistsAsync(bucketName);
        Assert.False(existsAfterDelete, "BucketExistsAsync returned true after deletion");
    }

    [Fact]
    public async Task FileOperations_ShouldWorkCorrectly()
    {
        // Arrange
        var service = CreateService();
        var bucketName = $"bucket-{Guid.NewGuid()}";
        var fileName = "test-file.txt";
        var content = "Hello RustFS!";
        using var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act & Assert
        // 1. Upload File (Implicitly creates bucket)
        var uploadResult = await service.UploadFileAsync(bucketName, fileName, uploadStream, "text/plain");
        Assert.True(uploadResult.Success, "UploadFileAsync failed");
        Assert.NotNull(uploadResult.Url);

        // 2. List Files
        var files = await service.ListFilesAsync(bucketName);
        Assert.Contains(fileName, files);

        // 3. Get File
        await using var downloadStream = await service.GetFileAsync(bucketName, fileName);
        using var reader = new StreamReader(downloadStream);
        var fileContent = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        Assert.Equal(content, fileContent);

        // 4. Delete File
        var deleteResult = await service.DeleteFileAsync(bucketName, fileName);
        // Note: RustFSService interface defines DeleteFileAsync but implementation might return void or bool, checking interface
        // IRustFSService.DeleteFileAsync returns Task<bool>
        Assert.True(deleteResult, "DeleteFileAsync failed");

        // 5. Verify Deletion
        // ListFilesAsync should not contain the file
        var filesAfterDelete = await service.ListFilesAsync(bucketName);
        Assert.DoesNotContain(fileName, filesAfterDelete);

        // GetFileAsync should throw Exception
        await Assert.ThrowsAnyAsync<AmazonS3Exception>(async () => await service.GetFileAsync(bucketName, fileName));
    }

    [Fact]
    public async Task GetFileAsync_ShouldThrow_WhenFileDoesNotExist()
    {
        // Arrange
        var service = CreateService();
        var bucketName = $"bucket-{Guid.NewGuid()}";
        var fileName = "non-existent-file.txt";

        // Create bucket first
        await service.CreateBucketAsync(bucketName);

        // Act & Assert
        await Assert.ThrowsAnyAsync<AmazonS3Exception>(async () => await service.GetFileAsync(bucketName, fileName));
    }

    [Fact]
    public async Task ListFilesAsync_ShouldReturnEmpty_WhenBucketDoesNotExist()
    {
        // Arrange
        var service = CreateService();
        var bucketName = $"non-existent-bucket-{Guid.NewGuid()}";

        // Act
        var files = await service.ListFilesAsync(bucketName);

        // Assert
        Assert.Empty(files);
    }
}
