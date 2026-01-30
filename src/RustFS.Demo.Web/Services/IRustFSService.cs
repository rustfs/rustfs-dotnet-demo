using RustFS.Demo.Web.Models;
using RustFS.Demo.Web.Options;

namespace RustFS.Demo.Web.Services;

/// <summary>
/// RustFS 服务接口 (兼容 S3)
/// </summary>
public interface IRustFSService
{
    #region 存储桶操作

    /// <summary>
    /// 创建存储桶
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>创建成功返回 true</returns>
    Task<bool> CreateBucketAsync(string bucketName);

    /// <summary>
    /// 检查存储桶是否存在
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>存在返回 true，否则返回 false</returns>
    Task<bool> BucketExistsAsync(string bucketName);

    /// <summary>
    /// 删除存储桶（会先清空其中的文件）
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>删除成功返回 true</returns>
    Task<bool> DeleteBucketAsync(string bucketName);

    /// <summary>
    /// 列出所有存储桶
    /// </summary>
    /// <returns>存储桶名称列表</returns>
    Task<IEnumerable<string>> ListBucketsAsync();

    #endregion

    #region 文件操作

    /// <summary>
    /// 生成预签名上传 URL
    /// </summary>
    /// <param name="options">生成选项</param>
    /// <returns>预签名 URL</returns>
    Task<string> GeneratePresignedUploadUrlAsync(PresignedUrlOptions options);

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <param name="key">文件键值（文件名）</param>
    /// <param name="fileStream">文件流</param>
    /// <param name="contentType">内容类型</param>
    /// <returns>上传结果</returns>
    Task<UploadResult> UploadFileAsync(string bucketName, string key, Stream fileStream, string contentType);

    /// <summary>
    /// 获取文件流
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <param name="key">文件键值（文件名）</param>
    /// <returns>文件流</returns>
    Task<Stream> GetFileAsync(string bucketName, string key);

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <param name="key">文件键值（文件名）</param>
    /// <returns>删除成功返回 true</returns>
    Task<bool> DeleteFileAsync(string bucketName, string key);

    /// <summary>
    /// 列出存储桶中的文件
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns>文件名列表</returns>
    Task<IEnumerable<string>> ListFilesAsync(string bucketName);

    #endregion
}

