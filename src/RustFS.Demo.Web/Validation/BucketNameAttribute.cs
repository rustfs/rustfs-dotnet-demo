using System.ComponentModel.DataAnnotations;

namespace RustFS.Demo.Web.Validation;

/// <summary>
/// 存储桶名称验证特性
/// </summary>
public class BucketNameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string bucketName)
        {
            return ValidationResult.Success; // 忽略非字符串类型，或者如果不是必填项
        }

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            return new ValidationResult("存储桶名称不能为空。");
        }

        // 检查长度
        if (bucketName.Length < 3 || bucketName.Length > 63)
        {
            return new ValidationResult("存储桶名称长度必须在 3 到 63 个字符之间。");
        }

        // 检查是否以字母或数字开头和结尾
        if (!char.IsLetterOrDigit(bucketName[0]) || !char.IsLetterOrDigit(bucketName[^1]))
        {
            return new ValidationResult("存储桶名称必须以字母或数字开头和结尾。");
        }

        // 检查字符有效性
        if (bucketName.Any(c => !(char.IsLower(c) || char.IsDigit(c) || c == '.' || c == '-')))
        {
            return new ValidationResult("存储桶名称只能包含小写字母、数字、点(.)和连字符(-)。");
        }

        // 检查连续的点
        if (bucketName.Contains(".."))
        {
            return new ValidationResult("存储桶名称不能包含连续的点(..)。");
        }

        // 检查是否是 IP 地址格式
        // 仅当包含点时才可能是通常意义上的 IP 地址格式 (x.x.x.x) 避免纯数字误判
        if (bucketName.Contains('.') && System.Net.IPAddress.TryParse(bucketName, out _))
        {
            return new ValidationResult("存储桶名称不能是 IP 地址格式。");
        }

        return ValidationResult.Success;
    }
}
