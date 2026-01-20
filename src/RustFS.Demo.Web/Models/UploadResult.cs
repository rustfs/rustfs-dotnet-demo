namespace RustFS.Demo.Web.Models;

/// <summary>
/// 文件上传结果
/// </summary>
/// <param name="Success">是否成功</param>
/// <param name="Url">文件访问地址</param>
/// <param name="ErrorMessage">错误信息（如果有）</param>
public record UploadResult(bool Success, string? Url = null, string? ErrorMessage = null);

