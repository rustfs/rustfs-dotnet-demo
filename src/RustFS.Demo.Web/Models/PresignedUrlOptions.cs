using System.ComponentModel.DataAnnotations;

namespace RustFS.Demo.Web.Models;

/// <summary>
/// 生成预签名 URL 选项
/// </summary>
/// <param name="Key">文件键值（文件名）</param>
/// <param name="ContentType">内容类型</param>
/// <param name="BucketName">存储桶名称</param>
/// <param name="DurationMinutes">有效期（分钟）</param>
public record PresignedUrlOptions(
    [Required(ErrorMessage = "Key cannot be empty")] string Key,
    string? ContentType = null,
    string? BucketName = null,
    double DurationMinutes = 10
);
