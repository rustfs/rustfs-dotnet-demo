using Microsoft.AspNetCore.Mvc;
using rustfs.dotnet.s3.demo.Models;

namespace rustfs.dotnet.s3.demo.Extensions;

/// <summary>
/// 控制器扩展方法
/// </summary>
internal static class ControllerExtensions
{
    /// <summary>
    /// 返回成功结果
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="controller">控制器</param>
    /// <param name="data">数据</param>
    /// <param name="message">消息</param>
    /// <returns>ActionResult</returns>
    public static IActionResult ApiOk<T>(this ControllerBase controller, T data, string message = "操作成功")
    {
        return controller.Ok(ApiResponse.Ok(data, message));
    }

    /// <summary>
    /// 返回成功结果（无数据）
    /// </summary>
    /// <param name="controller">控制器</param>
    /// <param name="message">消息</param>
    /// <returns>ActionResult</returns>
    public static IActionResult ApiOk(this ControllerBase controller, string message = "操作成功")
    {
        return controller.Ok(ApiResponse.OkWithoutData<object>(message));
    }

    /// <summary>
    /// 返回错误结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controller"></param>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <param name="code">错误码</param>
    /// <returns></returns>
    public static IActionResult ApiFail<T>(this ControllerBase controller, T data, string message, int code = 400)
    {
        return controller.Ok(ApiResponse.Fail(data, message, code));
    }

    /// <summary>
    /// 返回错误结果（无数据）
    /// </summary>
    /// <param name="controller">控制器</param>
    /// <param name="message">错误消息</param>
    /// <param name="code">错误码</param>
    /// <returns>ActionResult</returns>
    public static IActionResult ApiFail(this ControllerBase controller, string message, int code = 400)
    {
        return controller.Ok(ApiResponse.FailWithoutData<object>(message, code));
    }

    /// <summary>
    /// 返回自定义结果
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="controller">控制器</param>
    /// <param name="success">是否成功</param>
    /// <param name="code">状态码</param>
    /// <param name="message">消息</param>
    /// <param name="data">数据</param>
    /// <returns>ActionResult</returns>
    public static IActionResult ApiResult<T>(this ControllerBase controller, bool success, int code, string message, T? data = default)
    {
        var apiResp = new ApiResponse<T>(success, code, message, data);
        return controller.Ok(apiResp);
    }

    /// <summary>
    /// 为响应对象添加额外属性
    /// </summary>
    /// <param name="result">当前的操作结果</param>
    /// <param name="propertyName">待添加的属性名</param>
    /// <param name="value">待添加的属性值</param>
    /// <returns>添加了新属性的ActionResult</returns>
    public static IActionResult AddProperty(this IActionResult result, string propertyName, object? value)
    {
        if (result is not ObjectResult objectResult
            || objectResult.Value is not ApiResponse<object> response
            || string.IsNullOrWhiteSpace(propertyName))
            return result;

        if (response.Data is null)
            return result;

        // 如果已经有Data属性且为字典类型，则添加到字典中
        if (response.Data is IDictionary<string, object?> dict)
        {
            dict.TryAdd(propertyName, value);
            return result;
        }

        // 创建新字典并添加数据
        Dictionary<string, object?> newDict = [];
        newDict.Add("data", response.Data);
        newDict.TryAdd(propertyName, value);

        // 替换原始值
        objectResult.Value = newDict;

        return result;
    }

    /// <summary>
    /// 为响应添加额外属性到根对象中
    /// </summary>
    /// <param name="result">当前的操作结果</param>
    /// <param name="propertyName">待添加的属性名</param>
    /// <param name="value">待添加的属性值</param>
    /// <returns>添加了新属性的ActionResult</returns>
    public static IActionResult AddPropertyToRoot(this IActionResult result, string propertyName, object? value)
    {
        if (result is not ObjectResult objectResult
            || objectResult.Value == null
            || string.IsNullOrWhiteSpace(propertyName))
            return result;

        // 创建一个包含原始对象所有属性的匿名对象
        var originalProperties = objectResult.Value.GetType()
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(objectResult.Value));

        // 添加新属性
        originalProperties.TryAdd(propertyName, value);

        // 替换原始值
        objectResult.Value = originalProperties;

        return objectResult;
    }

}
