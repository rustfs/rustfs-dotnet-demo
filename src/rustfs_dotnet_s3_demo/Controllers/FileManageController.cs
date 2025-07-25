using rustfs.dotnet.s3.demo.Extensions;
using rustfs.dotnet.s3.demo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace rustfs.dotnet.s3.demo.Controllers;

[Route("api/file")]
[ApiController]
public class FileManageController(IAwsS3Service s3Service, ILogger<FileManageController> logger) : ControllerBase
{
    /// <summary>
    /// 上传文件到指定的存储桶
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <param name="key">文件键名</param>
    /// <param name="file">要上传的文件</param>
    /// <returns>上传结果</returns>
    [HttpPost("upload/{bucketName}/{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadFileAsync(
        [FromRoute] string bucketName,
        [FromRoute] string key,
        [FromForm] IFormFile file)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            var apiResp = this.ApiFail("Bucket name is required");
            return apiResp;
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            var apiResp = this.ApiFail("File key is required");
            return apiResp;
        }

        if (file == null || file.Length == 0)
        {
            var apiResp = this.ApiFail("No file uploaded");
            return apiResp;
        }

        using var stream = file.OpenReadStream();
        var result = await s3Service.UploadFileAsync(bucketName, key, stream, file.ContentType);
        if (result.Success)
        {
            KeyValuePair<string, string?> kvp = new("url", result.Url);
            var apiResp = this.ApiOk(kvp, "File uploaded successfully");
            return apiResp;
        }

        string error = result.ErrorMessage ?? "Internal service exception";
        return this.ApiFail(error, 500);
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <param name="key">文件键名</param>
    /// <returns>文件内容</returns>
    [HttpGet("download/{bucketName}/{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DownloadFileAsync(
        [FromRoute] string bucketName,
        [FromRoute] string key)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            var apiResp = this.ApiFail("Bucket name is required");
            return apiResp;
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            var apiResp = this.ApiFail("File key is required");
            return apiResp;
        }

        var exists = await s3Service.BucketExistsAsync(bucketName);
        if (!exists)
        {
            var apiResp = this.ApiFail($"Bucket '{bucketName}' does not exist", 404);
            return apiResp;
        }

        var fileStream = await s3Service.GetFileAsync(bucketName, key);
        if (fileStream == null)
        {
            var apiResp = this.ApiFail($"File '{key}' not found in bucket '{bucketName}'", 404);
            return apiResp;
        }

        string contentType = $"{MediaTypeNames.Application.Octet};charset=utf-8";
        return File(fileStream, contentType, key);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <param name="key">文件键名</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{bucketName}/{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteFile(
        [FromRoute] string bucketName,
        [FromRoute] string key)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            var apiResp = this.ApiFail("Bucket name is required");
            return apiResp;
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            var apiResp = this.ApiFail("File key is required");
            return apiResp;
        }

        var exists = await s3Service.BucketExistsAsync(bucketName);
        if (!exists)
        {
            var apiResp = this.ApiFail($"Bucket '{bucketName}' does not exist", 404);
            return apiResp;
        }

        var result = await s3Service.DeleteFileAsync(bucketName, key);

        return result ? this.ApiOk($"File '{key}' deleted successfully from bucket '{bucketName}'")
            : this.ApiFail($"File '{key}' not found in bucket '{bucketName}'", 404);
    }

    /// <summary>
    /// 列出存储桶中的所有文件
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>文件列表</returns>
    [HttpGet("{bucketName}/list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListFiles([FromRoute] string bucketName)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            var apiResp = this.ApiFail("Bucket name is required");
            return apiResp;
        }

        var exists = await s3Service.BucketExistsAsync(bucketName);
        if (!exists)
        {
            var apiResp = this.ApiFail($"Bucket '{bucketName}' does not exist", 404);
            return apiResp;
        }

        var files = await s3Service.ListFilesAsync(bucketName);
        KeyValuePair<string, IEnumerable<string>> kvp = new("files", files);

        return this.ApiOk(kvp, "ok");
    }

}
