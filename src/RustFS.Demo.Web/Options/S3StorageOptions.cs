namespace RustFS.Demo.Web.Options;

public class S3StorageOptions
{
    // 配置键常量
    public const string SectionName = "S3Storage";

    #region 配置项
    public Uri? Endpoint { get; set; }

    public string Region { get; set; } = "us-east-1";

    public bool UseInstanceProfile { get; set; }

    public string? AssumeRoleArn { get; set; }

    public string? AccessKey { get; set; }

    public string? SecretKey { get; set; } 
    #endregion

    // 验证配置项有效性
    public bool IsValid() =>
        !string.IsNullOrWhiteSpace(AccessKey) &&
        !string.IsNullOrWhiteSpace(SecretKey) &&
        Endpoint != null;

}
