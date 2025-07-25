using Microsoft.AspNetCore.Mvc;
using rustfs.dotnet.s3.demo.Extensions;
using rustfs.dotnet.s3.demo.Services;

namespace rustfs.dotnet.s3.demo.Controllers;

[Route("api/bucket")]
[ApiController]
public class BucketManageController(
    ILogger<BucketManageController> logger,
    IAwsS3Service s3Service) : ControllerBase
{
    /// <summary>
    /// 创建存储桶
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>创建结果</returns>
    [HttpPost("{bucketName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateBucketAsync([FromRoute] string bucketName)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return this.ApiFail("Bucket name is required");

        // 验证存储桶名称
        if (!IsValidBucketName(bucketName))
            return this.ApiFail("Invalid bucket name. Bucket names must be between 3 and 63 characters long, and can contain only lowercase letters, numbers, dots, and hyphens.");

        var exists = await s3Service.BucketExistsAsync(bucketName);
        if (exists)
            return this.ApiFail($"Bucket '{bucketName}' already exists");

        var result = await s3Service.CreateBucketAsync(bucketName);
        return result ? this.ApiOk($"Bucket '{bucketName}' created successfully")
                : this.ApiFail("Failed to create bucket", 500);
    }

    /// <summary>
    /// 检查存储桶是否存在
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>存储桶是否存在</returns>
    [HttpGet("{bucketName}/exists")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BucketExistsAsync([FromRoute] string bucketName)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return this.ApiFail("Bucket name is required");

        var exists = await s3Service.BucketExistsAsync(bucketName);
        return exists ? this.ApiOk($"Bucket '{bucketName}' already exists")
            : this.ApiFail($"Bucket '{bucketName}' does not exist");
    }

    /// <summary>
    /// 删除存储桶
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{bucketName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteBucketAsync([FromRoute] string bucketName)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
            return this.ApiFail("Bucket name is required");

        var exists = await s3Service.BucketExistsAsync(bucketName);
        if (!exists)
            return this.ApiFail($"Bucket '{bucketName}' does not exist", 404);

        var result = await s3Service.DeleteBucketAsync(bucketName);
        return result ? this.ApiOk($"Bucket '{bucketName}' deleted successfully")
            : this.ApiFail("Failed to delete bucket", 500);
    }

    /// <summary>
    /// 列出所有存储桶
    /// </summary>
    /// <returns>存储桶列表</returns>
    [HttpGet("list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListBucketsAsync()
    {
        var buckets = await s3Service.ListBucketsAsync();
        return this.ApiOk(buckets);
    }

    // 验证存储桶名称
    private bool IsValidBucketName(string bucketName)
    {
        // 检查长度
        if (bucketName.Length < 3 || bucketName.Length > 63)
            return false;

        // 检查是否以字母或数字开头和结尾
        if (!char.IsLetterOrDigit(bucketName[0]) || !char.IsLetterOrDigit(bucketName[bucketName.Length - 1]))
            return false;

        // 检查字符有效性
        for (int i = 0; i < bucketName.Length; i++)
        {
            char c = bucketName[i];
            if (!(char.IsLower(c) || char.IsDigit(c) || c == '.' || c == '-'))
                return false;
        }

        // 检查连续的点
        if (bucketName.Contains(".."))
            return false;

        // 检查是否是 IP 地址格式
        if (System.Net.IPAddress.TryParse(bucketName, out _))
            return false;

        return true;
    }

}
