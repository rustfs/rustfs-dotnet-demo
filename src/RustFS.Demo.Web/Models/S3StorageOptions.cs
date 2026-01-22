namespace RustFS.Demo.Web.Models;

public class S3StorageOptions
{
    public Uri? Endpoint { get; set; }

    public string Region { get; set; } = "us-east-1";

    public bool UseInstanceProfile { get; set; }

    public string? AssumeRoleArn { get; set; }

    public string? AccessKey { get; set; }

    public string? SecretKey { get; set; }
}

