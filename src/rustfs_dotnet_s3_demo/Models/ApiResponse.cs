using System.Text.Json.Serialization;

namespace rustfs.dotnet.s3.demo.Models;

/// <summary>
/// API响应工厂类
/// </summary>
public static class ApiResponse
{
    /// <summary>
    /// 创建成功响应(无数据data)
    /// </summary>
    public static ApiResponse<TData> OkWithoutData<TData>(string message = "操作成功")
        => new(true, 200, message, default);

    /// <summary>
    /// 创建成功响应(有数据data)
    /// </summary>
    public static ApiResponse<TData> Ok<TData>(TData data, string message = "操作成功")
        => new(true, 200, message, data);

    /// <summary>
    /// 创建失败响应(无数据data)
    /// </summary>
    public static ApiResponse<TData> FailWithoutData<TData>(string message, int code = 400)
        => new(false, code, message, default);

    /// <summary>
    /// 创建失败响应(有数据data)
    /// </summary>
    public static ApiResponse<TData> Fail<TData>(TData data, string message, int code = 400)
        => new(false, code, message, data);

    /// <summary>
    /// 创建 Problem Details 格式的错误响应
    /// </summary>
    public static ProblemDetailsResponse CreateProblemDetails(
        string title,
        int status,
        string? detail = null,
        string? instance = null,
        IDictionary<string, object?>? extensions = null)
        => new(title, status, detail, instance, extensions);
}

/// <summary>
/// 通用API响应结构
/// </summary>
/// <typeparam name="TData">返回数据</typeparam>
/// <param name="Success"></param>
/// <param name="Code"></param>
/// <param name="Message"></param>
/// <param name="Data"></param>
public record ApiResponse<TData>(bool Success, int Code, string Message, TData? Data)
{
    public bool Success { get; init; } = Success;
    public int Code { get; init; } = Code;
    public string Message { get; init; } = Message;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TData? Data { get; init; } = Data;
}

/// <summary>
/// Problem Details 响应结构 (RFC 7807)
/// </summary>
public record ProblemDetailsResponse(
    string Title,
    int Status,
    string? Detail = null,
    string? Instance = null,
    IDictionary<string, object?>? Extensions = null)
{
    public string Type { get; init; } = "about:rustfs";
    public string Title { get; init; } = Title;
    public int Status { get; init; } = Status;
    public string? Detail { get; init; } = Detail;
    public string? Instance { get; init; } = Instance;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, object?>? Extensions { get; init; } = Extensions;

    public DateTime Timestamp => DateTime.UtcNow;
}
