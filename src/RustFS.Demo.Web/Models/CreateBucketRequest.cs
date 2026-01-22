using RustFS.Demo.Web.Validation;

namespace RustFS.Demo.Web.Models;

/// <summary>
/// 创建存储桶请求
/// </summary>
public record CreateBucketRequest
{
    /// <summary>
    /// 存储桶名称
    /// </summary>
    [BucketName]
    public string Name { get; init; } = string.Empty;
}
