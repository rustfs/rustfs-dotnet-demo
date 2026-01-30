using Microsoft.AspNetCore.Mvc;
using RustFS.Demo.Web.Models;
using RustFS.Demo.Web.Options;
using RustFS.Demo.Web.Services;
using RustFS.Demo.Web.Validation;

namespace RustFS.Demo.Web.Controllers;

/// <summary>
/// 文件管理控制器
/// </summary>
[ApiController]
[Route("api/buckets/{bucketName}/files")]
public sealed class FileController(
    ILogger<FileController> logger, 
    IRustFSService s3Service) : ControllerBase
{
    /// <summary>
    /// 获取存储桶中的文件列表
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>文件名列表</returns>
    [HttpGet]
    public async Task<IEnumerable<string>> ListFiles([BucketName] string bucketName)
    {
        if (!await s3Service.BucketExistsAsync(bucketName))
            throw new KeyNotFoundException("Bucket not found");

        return await s3Service.ListFilesAsync(bucketName);
    }

    /// <summary>
    /// 获取预签名上传 URL
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <param name="options">请求参数</param>
    /// <returns>预签名 URL</returns>
    [HttpGet("presigned-upload-url")]
    public async Task<ActionResult<string>> GetPresignedUploadUrl([BucketName] string bucketName, [FromQuery] PresignedUrlOptions options)
    {
        var finalOptions = options with { BucketName = bucketName, ContentType = options.ContentType ?? "application/octet-stream" };
        var url = await s3Service.GeneratePresignedUploadUrlAsync(finalOptions);
        return Ok(new { url });
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
        var result = await s3Service.UploadFileAsync(bucketName, file.FileName, stream, file.ContentType);

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
        await s3Service.DeleteFileAsync(bucketName, key);
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
        var stream = await s3Service.GetFileAsync(bucketName, key);
        return File(stream, "application/octet-stream", key);
    }
}
