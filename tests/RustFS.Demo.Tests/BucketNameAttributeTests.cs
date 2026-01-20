using System.ComponentModel.DataAnnotations;
using RustFS.Demo.Web.Validation;

namespace RustFS.Demo.Tests;

public class BucketNameAttributeTests
{
    private readonly BucketNameAttribute _attribute = new();

    [Theory]
    [InlineData("my-bucket")]
    [InlineData("my.bucket")]
    [InlineData("123-bucket")]
    [InlineData("bucket-123")]
    [InlineData("a.b.c")]
    [InlineData("my-bucket-123")]
    [InlineData("123456")]
    public void IsValid_ValidBucketNames_ReturnsSuccess(string bucketName)
    {
        // Act
        var result = _attribute.GetValidationResult(bucketName, new ValidationContext(new object()));

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Theory]
    [InlineData("ab", "存储桶名称长度必须在 3 到 63 个字符之间。")] // Length < 3
    [InlineData("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijkl", "存储桶名称长度必须在 3 到 63 个字符之间。")] // Length > 63 (64 chars)
    [InlineData("-start-with-hyphen", "存储桶名称必须以字母或数字开头和结尾。")]
    [InlineData("end-with-hyphen-", "存储桶名称必须以字母或数字开头和结尾。")]
    [InlineData(".start-with-dot", "存储桶名称必须以字母或数字开头和结尾。")]
    [InlineData("end-with-dot.", "存储桶名称必须以字母或数字开头和结尾。")]
    [InlineData("Upper-Case", "存储桶名称只能包含小写字母、数字、点(.)和连字符(-)。")]
    [InlineData("invalid!char", "存储桶名称只能包含小写字母、数字、点(.)和连字符(-)。")]
    [InlineData("bucket..name", "存储桶名称不能包含连续的点(..)。")]
    [InlineData("192.168.1.1", "存储桶名称不能是 IP 地址格式。")]
    [InlineData("", "存储桶名称不能为空。")]
    [InlineData("   ", "存储桶名称不能为空。")]
    public void IsValid_InvalidBucketNames_ReturnsErrorMessage(string bucketName, string expectedErrorMessage)
    {
        // Act
        var result = _attribute.GetValidationResult(bucketName, new ValidationContext(new object()));

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal(expectedErrorMessage, result.ErrorMessage);
    }

    [Fact]
    public void IsValid_NonStringValue_ReturnsSuccess()
    {
        // Act
        // 验证特性通常只验证特定类型，对于非目标类型通常返回 Success (除非强制类型检查，但这里是 `value is not string` return Success)
        var result = _attribute.GetValidationResult(123, new ValidationContext(new object()));

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void IsValid_NullValue_ReturnsSuccess()
    {
        // Act
        // Null 值通常由 [Required] 特性处理，自定义验证器如果收到 null 通常应该通过（除非它隐含 Required）
        // 这里的代码逻辑是 `if (value is not string bucketName) return ValidationResult.Success;`
        var result = _attribute.GetValidationResult(null, new ValidationContext(new object()));

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }
}
