using Amazon.S3.Model;

namespace RustFS.Demo.Web.Services;

public partial class RustFSService
{
    /// <summary>
    /// 创建存储桶
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>创建成功返回 true</returns>
    public async Task<bool> CreateBucketAsync(string bucketName)
    {
        bool hasBucket = await BucketExistsAsync(bucketName);
        if (hasBucket)
            return true;

        PutBucketRequest request = new()
        {
            BucketName = bucketName,
            UseClientRegion = false
        };

        await _s3Client.PutBucketAsync(request);
        return true;
    }

    /// <summary>
    /// 检查存储桶是否存在
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>存在返回 true，否则返回 false</returns>
    public async Task<bool> BucketExistsAsync(string bucketName)
    {
        var response = await _s3Client.ListBucketsAsync();
        return response.Buckets?.Any(b => b.BucketName == bucketName) ?? false;
    }

    /// <summary>
    /// 删除存储桶（会先清空其中的文件）
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>删除成功返回 true</returns>
    public async Task<bool> DeleteBucketAsync(string bucketName)
    {
        // 首先删除存储桶中的所有对象
        var files = await ListFilesAsync(bucketName);
        if (files.Any())
        {
            DeleteObjectsRequest deleteObjectsRequest = new()
            {
                BucketName = bucketName,
                Objects = files.Select(k => new KeyVersion { Key = k }).ToList()
            };

            await _s3Client.DeleteObjectsAsync(deleteObjectsRequest);
        }

        // 删除存储桶
        DeleteBucketRequest deleteBucketRequest = new()
        {
            BucketName = bucketName
        };

        await _s3Client.DeleteBucketAsync(deleteBucketRequest);
        return true;
    }

    /// <summary>
    /// 列出所有存储桶
    /// </summary>
    /// <returns>存储桶名称列表</returns>
    public async Task<IEnumerable<string>> ListBucketsAsync()
    {
        var response = await _s3Client.ListBucketsAsync();
        return response.Buckets?.Select(b => b.BucketName) ?? Enumerable.Empty<string>();
    }

}
