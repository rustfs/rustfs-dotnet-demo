using Amazon.S3;
using Amazon.S3.Model;

namespace rustfs.dotnet.s3.demo.Services;

public partial class AwsS3Service
{
    #region 存储桶操作
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

        var response = await _s3client.PutBucketAsync(request);
        return true;
    }

    public async Task<bool> BucketExistsAsync(string bucketName)
    {
        ListObjectsV2Request request = new()
        {
            BucketName = bucketName,
            MaxKeys = 1
        };

        var resp = await _s3client.ListObjectsV2Async(request);
        if (resp.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            if (resp.S3Objects != null && resp.S3Objects.Any())
                return true;
        }

        return false;
    }

    public async Task<bool> DeleteBucketAsync(string bucketName)
    {
        // 首先删除存储桶中的所有对象
        ListObjectsV2Request listRequest = new()
        {
            BucketName = bucketName
        };

        var listResponse = await _s3client.ListObjectsV2Async(listRequest);
        if (listResponse.S3Objects.Any())
        {
            DeleteObjectsRequest deleteObjectsRequest = new()
            {
                BucketName = bucketName,
                Objects = listResponse.S3Objects.Select(o => new KeyVersion { Key = o.Key }).ToList()
            };

            await _s3client.DeleteObjectsAsync(deleteObjectsRequest);
        }

        // 删除存储桶
        DeleteBucketRequest deleteBucketRequest = new()
        {
            BucketName = bucketName
        };

        await _s3client.DeleteBucketAsync(deleteBucketRequest);
        return true;
    }

    public async Task<IEnumerable<string>> ListBucketsAsync()
    {
        var response = await _s3client.ListBucketsAsync();
        return response.Buckets.Select(b => b.BucketName);
    }

    #endregion

}
