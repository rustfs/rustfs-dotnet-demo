using RustFS.Demo.Web.Models;

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
    /// <param name="bucketName"></param>
    /// <returns></returns>
    Task<bool> CreateBucketAsync(string bucketName);

    /// <summary>
    /// 检查存储桶是否存在
    /// </summary>
    /// <param name="bucketName"></param>
    /// <returns></returns>
    Task<bool> BucketExistsAsync(string bucketName);

    /// <summary>
    /// 删除存储桶
    /// </summary>
    /// <param name="bucketName"></param>
    /// <returns></returns>
    Task<bool> DeleteBucketAsync(string bucketName);

    /// <summary>
    /// 列出所有存储桶
    /// </summary>
    /// <returns></returns>
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
    /// <param name="bucketName"></param>
    /// <param name="key"></param>
    /// <param name="fileStream"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    Task<UploadResult> UploadFileAsync(string bucketName, string key, Stream fileStream, string contentType);

    /// <summary>
    /// 获取文件
    /// </summary>
    /// <param name="bucketName"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<Stream> GetFileAsync(string bucketName, string key);

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="bucketName"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<bool> DeleteFileAsync(string bucketName, string key);

    /// <summary>
    /// 列出存储桶中的文件
    /// </summary>
    /// <param name="bucketName"></param>
    /// <returns></returns>
    Task<IEnumerable<string>> ListFilesAsync(string bucketName);

    #endregion
}

