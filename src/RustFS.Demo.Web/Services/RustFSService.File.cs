using RustFS.Demo.Web.Models;
using RustFS.Demo.Web.Options;

namespace RustFS.Demo.Web.Services;

public partial class RustFSService
{
    public async Task<string> GeneratePresignedUploadUrlAsync(PresignedUrlOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.BucketName))
        {
            throw new ArgumentException("BucketName cannot be empty", nameof(options));
        }

        // 检查存储桶是否存在，如果不存在则创建
        if (!await BucketExistsAsync(options.BucketName))
        {
            await CreateBucketAsync(options.BucketName);
        }

        GetPreSignedUrlRequest request = new()
        {
            BucketName = options.BucketName,
            Key = options.Key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(options.DurationMinutes),
            ContentType = options.ContentType ?? MediaTypeNames.Application.Octet,
            Protocol = _serviceUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase) ? Protocol.HTTPS : Protocol.HTTP
        };

        return await s3Client.GetPreSignedURLAsync(request);
    }

    public async Task<UploadResult> UploadFileAsync(string bucketName, string key, Stream fileStream, string contentType)
    {
        // 检查存储桶是否存在，如果不存在则创建
        if (!await BucketExistsAsync(bucketName))
        {
            await CreateBucketAsync(bucketName);
        }

        PutObjectRequest request = new()
        {
            BucketName = bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType ?? MediaTypeNames.Application.Octet
        };

        await s3Client.PutObjectAsync(request);

        // 构建文件访问 URL (仅用于显示，可能需要预签名 URL 或公开访问配置)
        // 这里简单拼接
        var url = $"{_serviceUrl}/{bucketName}/{key}";
        return new UploadResult(true, url);
    }

    public async Task<Stream> GetFileAsync(string bucketName, string key)
    {
        GetObjectRequest request = new()
        {
            BucketName = bucketName,
            Key = key
        };

        var response = await s3Client.GetObjectAsync(request);
        return response.ResponseStream;
    }

    public async Task<bool> DeleteFileAsync(string bucketName, string key)
    {
        DeleteObjectRequest request = new()
        {
            BucketName = bucketName,
            Key = key
        };

        var response = await s3Client.DeleteObjectAsync(request);
        // S3 删除对象即使对象不存在也会返回成功（204 No Content）
        return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent || response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<IEnumerable<string>> ListFilesAsync(string bucketName)
    {
        // 检查 Bucket 是否存在，避免 ListObjectsV2Async 抛出 NotFound 异常
        if (!await BucketExistsAsync(bucketName))
        {
            return Enumerable.Empty<string>();
        }

        ListObjectsV2Request request = new()
        {
            BucketName = bucketName
        };

        var response = await s3Client.ListObjectsV2Async(request);
        return response.S3Objects?.Select(o => o.Key) ?? Enumerable.Empty<string>();
    }
}
