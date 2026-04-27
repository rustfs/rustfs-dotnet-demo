using Amazon.S3.Model;
using Amazon.S3.Util;

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
        _logger.LogStartingCreateBucket(bucketName);

        bool hasBucket = await BucketExistsAsync(bucketName);
        if (hasBucket)
        {
            _logger.LogBucketAlreadyExists(bucketName);
            return true;
        }

        PutBucketRequest request = new()
        {
            BucketName = bucketName,
            UseClientRegion = false
        };

        await _s3Client.PutBucketAsync(request);
        _logger.LogBucketCreated(bucketName);
        return true;
    }

    /// <summary>
    /// 检查存储桶是否存在
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>存在返回 true，否则返回 false</returns>
    public async Task<bool> BucketExistsAsync(string bucketName)
    {
        var exists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
        _logger.LogCheckingBucketExists(bucketName, exists);
        return exists;
    }

    /// <summary>
    /// 删除存储桶（会先清空其中的文件）
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>删除成功返回 true</returns>
    public async Task<bool> DeleteBucketAsync(string bucketName)
    {
        _logger.LogStartingDeleteBucket(bucketName);

        // 使用 AmazonS3Util 自动处理对象删除和存储桶删除
        await AmazonS3Util.DeleteS3BucketWithObjectsAsync(_s3Client, bucketName);

        _logger.LogBucketDeleted(bucketName);
        return true;
    }

    /// <summary>
    /// 列出所有存储桶
    /// </summary>
    /// <returns>存储桶名称列表</returns>
    public async Task<IEnumerable<string>> ListBucketsAsync()
    {
        var response = await _s3Client.ListBucketsAsync();
        var count = response.Buckets?.Count ?? 0;
        _logger.LogBucketsRetrieved(count);
        return response.Buckets?.Select(b => b.BucketName) ?? Enumerable.Empty<string>();
    }

}
