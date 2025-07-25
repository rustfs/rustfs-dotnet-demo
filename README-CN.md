

# RustFS .NET S3 演示项目

<p align="center"> <a href="https://github.com/rustfs/rustfs-dotnet-demo/"> English </a> |  中文 </p>

这是一个使用 `RustFS` 技术实现的 `.NET S3` 演示项目，展示了如何在 `.NET` 环境中与 `S3` 兼容的存储服务进行交互。

## 功能概述

本项目主要实现了以下功能：
- 存储桶管理：创建、删除、检查存在性、列出所有存储桶
- 文件管理：上传、下载、删除、列出存储桶中的文件
- 统一的 `API` 响应格式 `ApiResponse`
- 异常处理中间件

## 主要特性

- 使用 `AWS S3` 兼容的 `API` 进行对象存储操作
- 清晰的错误处理和响应格式
- 模块化的代码结构
- 支持异步操作

## API 规范

所有 `API` 都遵循统一的响应格式，包含以下字段：
- `Success`: 操作是否成功
- `Code`: HTTP 状态码
- `Message`: 操作结果描述
- `Data`: 返回数据（如果有）

## 环境说明

- 安装 `.NET9 SDK 9.0.7` 及最新版本，命令查看环境信息：

```bash
dotnet --info
```

- 本地安装 `Docker` 环境，启动 `RustFS` 容器服务：

```bash
# 拉取镜像
docker pull rustfs/rustfs:latest
# 启动容器
docker run -d \
  --name rustfs \
  -p 9000:9000 \
  -v /data:/data \
  rustfs/rustfs:latest
```

更多信息请查看：

- `https://rustfs.com/`
- `https://dotnet.microsoft.com/zh-cn/download`


## 使用示例

### 启动项目

- 使用 `cmd/powershell/pwsh` 终端运行以下命令：

```pwsh
# 克隆项目
git clone https://gitee.com/dolayout/rustfs_dotnet.git
# 进入项目根目录
cd rustfs_dotnet
# 还原 nuget 包
dotnet restore
# 运行项目
dotnet run -c Release -p ./src/rustfs_dotnet_s3_demo
```

- 浏览器输入地址：

```bash
http://localhost:5073/scalar/v1
```

### 接口操作

- 创建存储桶

```http
POST /api/bucket/{bucketName}
```

- 上传文件

```http
POST /api/file/upload/{bucketName}/{key}
Form-data: file=@local_file_path
```

- 下载文件

```http
GET /api/file/download/{bucketName}/{key}
```

- 删除文件

```http
DELETE /api/file/{bucketName}/{key}
```

## 项目结构

- `Controllers`: 包含 `BucketManageController.cs` 和 `FileManageController.cs` 等控制器类
- `Extensions`: 包含控制器扩展方法
- `Handlers`: 包含异常处理相关类
- `Models`: 包含响应模型类
- `Services`: 包含 `AWS S3` 服务实现
- `Settings`: 包含 `AWS S3` 配置设置

## 错误处理

项目使用统一的异常处理机制，所有异常都会被转换为标准化 `ProblemDetailsResponse` 格式返回给客户端。

## 配置

`AWS S3` 的连接信息通过 `AwsS3Setting` 类进行配置，包含服务地址、访问密钥、秘密密钥和区域设置。

## 许可证

本项目遵循 `MIT` 许可证。

## 特别鸣谢

👉 特别感谢 <a href="https://github.com/zfchai">@zfchai</a> 提交的关于.Net的第一版演示代码。 

