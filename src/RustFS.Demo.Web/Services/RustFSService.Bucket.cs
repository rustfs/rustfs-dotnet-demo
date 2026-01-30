namespace RustFS.Demo.Web.Services;

public partial class RustFSService
{
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

        await s3Client.PutBucketAsync(request);
        return true;
    }

    public async Task<bool> BucketExistsAsync(string bucketName)
    {
        var response = await s3Client.ListBucketsAsync();
        return response.Buckets?.Any(b => b.BucketName == bucketName) ?? false;
    }

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

            await s3Client.DeleteObjectsAsync(deleteObjectsRequest);
        }

        // 删除存储桶
        DeleteBucketRequest deleteBucketRequest = new()
        {
            BucketName = bucketName
        };

        await s3Client.DeleteBucketAsync(deleteBucketRequest);
        return true;
    }

    public async Task<IEnumerable<string>> ListBucketsAsync()
    {
        var response = await s3Client.ListBucketsAsync();
        return response.Buckets?.Select(b => b.BucketName) ?? [];
    }

}
