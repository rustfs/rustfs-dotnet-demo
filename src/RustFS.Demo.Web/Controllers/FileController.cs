using Microsoft.AspNetCore.Mvc;
using RustFS.Demo.Web.Models;
using RustFS.Demo.Web.Services;
using RustFS.Demo.Web.Validation;

namespace RustFS.Demo.Web.Controllers;

/// <summary>
/// 文件管理控制器
/// </summary>
[ApiController]
[Route("api/buckets/{bucketName}/files")]
public class FileController : ControllerBase
{
    private readonly IRustFSService _s3Service;
    private readonly ILogger<FileController> _logger;

    public FileController(IRustFSService s3Service, ILogger<FileController> logger)
    {
        _s3Service = s3Service;
        _logger = logger;
    }

    /// <summary>
    /// 获取存储桶中的文件列表
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>文件名列表</returns>
    [HttpGet]
    public async Task<IEnumerable<string>> ListFiles([BucketName] string bucketName)
    {
        if (!await _s3Service.BucketExistsAsync(bucketName))
            throw new KeyNotFoundException("Bucket not found");

        return await _s3Service.ListFilesAsync(bucketName);
    }

    /// <summary>
    /// 上传文件到指定存储桶
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <param name="file">要上传的文件</param>
    /// <returns>上传结果</returns>
    [HttpPost]
    public async Task<ActionResult<UploadResult>> UploadFile([BucketName] string bucketName, IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file uploaded");

        await using var stream = file.OpenReadStream();
        var result = await _s3Service.UploadFileAsync(bucketName, file.FileName, stream, file.ContentType);

        if (result.Success)
            return Ok(result);

        throw new InvalidOperationException(result.ErrorMessage);
    }

    /// <summary>
    /// 从存储桶删除指定文件
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <param name="key">文件键值（文件名）</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{key}")]
    public async Task<IActionResult> DeleteFile([BucketName] string bucketName, string key)
    {
        await _s3Service.DeleteFileAsync(bucketName, key);
        return NoContent();
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <param name="key">文件键值（文件名）</param>
    /// <returns>文件流</returns>
    [HttpGet("{key}")]
    public async Task<FileStreamResult> DownloadFile([BucketName] string bucketName, string key)
    {
        var stream = await _s3Service.GetFileAsync(bucketName, key);
        return File(stream, "application/octet-stream", key);
    }
}
