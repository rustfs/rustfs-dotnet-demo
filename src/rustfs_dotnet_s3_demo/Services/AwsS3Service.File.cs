using rustfs.dotnet.s3.demo.Models;
using Amazon.S3;
using Amazon.S3.Model;
using System.Net.Mime;

namespace rustfs.dotnet.s3.demo.Services;

public partial class AwsS3Service
{
    #region 文件操作
    // 上传文件
    public async Task<UploadResult> UploadFileAsync(string bucketName, string key, Stream fileStream, string contentType)
    {
        try
        {
            // 检查存储桶是否存在，如果不存在则创建
            if (!await BucketExistsAsync(bucketName))
            {
                var bucketCreated = await CreateBucketAsync(bucketName);
                if (!bucketCreated)
                {
                    return new UploadResult(false, ErrorMessage: "Failed to create bucket");
                }
            }

            PutObjectRequest request = new()
            {
                BucketName = bucketName,
                Key = key,
                InputStream = fileStream,
                ContentType = contentType ?? MediaTypeNames.Application.Octet
            };

            var response = await _s3client.PutObjectAsync(request);

            // 构建文件访问 URL
            var url = $"{_settings.ServiceUrl}/{bucketName}/{key}";
            return new UploadResult(true, url);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading file: {ex.Message}");
            return new UploadResult(false, ErrorMessage: ex.Message);
        }
    }

    // 获取文件
    public async Task<Stream> GetFileAsync(string bucketName, string key)
    {
        GetObjectRequest request = new()
        {
            BucketName = bucketName,
            Key = key
        };

        var response = await _s3client.GetObjectAsync(request);
        return response.ResponseStream;
    }

    // 删除文件
    public async Task<bool> DeleteFileAsync(string bucketName, string key)
    {
        DeleteObjectRequest request = new()
        {
            BucketName = bucketName,
            Key = key
        };

        var response = await _s3client.DeleteObjectAsync(request);
        return string.Equals(response.DeleteMarker, "true", StringComparison.OrdinalIgnoreCase);
    }

    // 列出存储桶中的文件
    public async Task<IEnumerable<string>> ListFilesAsync(string bucketName)
    {
        ListObjectsV2Request request = new()
        {
            BucketName = bucketName
        };

        var response = await _s3client.ListObjectsV2Async(request);
        return response.S3Objects.Select(o => o.Key);
    }
    #endregion
}
