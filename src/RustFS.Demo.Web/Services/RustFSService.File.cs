using Amazon.S3.Model;
using RustFS.Demo.Web.Models;
using System.Net.Mime;

namespace RustFS.Demo.Web.Services;

public partial class RustFSService
{
    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <param name="key">文件键值（文件名）</param>
    /// <param name="fileStream">文件流</param>
    /// <param name="contentType">内容类型</param>
    /// <returns>上传结果</returns>
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

        await _s3Client.PutObjectAsync(request);

        // 构建文件访问 URL (仅用于显示，可能需要预签名 URL 或公开访问配置)
        // 这里简单拼接
        var url = $"{_serviceUrl}/{bucketName}/{key}";
        return new UploadResult(true, url);
    }

    /// <summary>
    /// 获取文件流
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <param name="key">文件键值（文件名）</param>
    /// <returns>文件流</returns>
    public async Task<Stream> GetFileAsync(string bucketName, string key)
    {
        GetObjectRequest request = new()
        {
            BucketName = bucketName,
            Key = key
        };

        var response = await _s3Client.GetObjectAsync(request);
        return response.ResponseStream;
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <param name="key">文件键值（文件名）</param>
    /// <returns>删除成功返回 true</returns>
    public async Task<bool> DeleteFileAsync(string bucketName, string key)
    {
        DeleteObjectRequest request = new()
        {
            BucketName = bucketName,
            Key = key
        };

        var response = await _s3Client.DeleteObjectAsync(request);
        // S3 删除对象即使对象不存在也会返回成功（204 No Content）
        return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent || response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    /// <summary>
    /// 列出存储桶中的文件
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>文件名列表</returns>
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

        var response = await _s3Client.ListObjectsV2Async(request);
        return response.S3Objects?.Select(o => o.Key) ?? Enumerable.Empty<string>();
    }
}
