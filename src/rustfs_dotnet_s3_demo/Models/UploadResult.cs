namespace rustfs.dotnet.s3.demo.Models;

/// <summary>
/// 文件上传结果
/// </summary>
/// <param name="Success"></param>
/// <param name="Url"></param>
/// <param name="ErrorMessage"></param>
public record UploadResult(bool Success, string? Url = null, string? ErrorMessage = null);
