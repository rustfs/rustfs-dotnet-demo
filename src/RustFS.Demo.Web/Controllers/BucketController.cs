using Microsoft.AspNetCore.Mvc;
using RustFS.Demo.Web.Models;
using RustFS.Demo.Web.Services;
using RustFS.Demo.Web.Validation;

namespace RustFS.Demo.Web.Controllers;

/// <summary>
/// 存储桶管理控制器
/// </summary>
[ApiController]
[Route("api/buckets")]
public class BucketController : ControllerBase
{
    private readonly IRustFSService _s3Service;
    private readonly ILogger<BucketController> _logger;

    public BucketController(IRustFSService s3Service, ILogger<BucketController> logger)
    {
        _s3Service = s3Service;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有存储桶列表
    /// </summary>
    /// <returns>存储桶名称列表</returns>
    [HttpGet]
    public async Task<IEnumerable<string>> List()
    {
        return await _s3Service.ListBucketsAsync();
    }

    /// <summary>
    /// 创建新存储桶
    /// </summary>
    /// <param name="request">创建请求</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBucketRequest request)
    {
        await _s3Service.CreateBucketAsync(request.Name);
        return Ok();
    }

    /// <summary>
    /// 删除存储桶
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{bucketName}")]
    public async Task<IActionResult> Delete([BucketName] string bucketName)
    {
        await _s3Service.DeleteBucketAsync(bucketName);
        return NoContent();
    }
}
