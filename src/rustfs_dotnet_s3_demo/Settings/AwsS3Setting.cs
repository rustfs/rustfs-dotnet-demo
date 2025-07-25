namespace rustfs.dotnet.s3.demo.Settings;

public sealed class AwsS3Setting
{
    public const string Section = "RustFS";

    /// <summary>
    /// S3 兼容服务的地址，如 http://localhost:9000
    /// </summary>
    public string ServiceUrl { get; set; } = string.Empty;

    /// <summary>
    /// 访问密钥 ID，用于身份验证
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// 秘密访问密钥，用于身份验证
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// 区域标识，对于 RustFS 可以任意设置
    /// </summary>
    public string Region { get; set; } = "us-east-1";
}