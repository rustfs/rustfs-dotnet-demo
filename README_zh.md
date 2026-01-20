# RustFS.Demo

[![CI](https://github.com/rustfs/rustfs-dotnet-demo/actions/workflows/ci.yml/badge.svg)](https://github.com/rustfs/rustfs-dotnet-demo/actions/workflows/ci.yml)

[English](./README.md) | 中文

一个基于 **.NET Aspire** 的演示项目，展示了如何编排 **RustFS**（高性能 S3 兼容对象存储）和 **ASP.NET Core Web API**。

## 🚀 技术栈

- **.NET 10**
- **.NET Aspire**
- **RustFS** (S3 兼容对象存储)
- **Docker**

## 🛠️ 快速开始

1. **前置要求**: 确保已安装 [Docker Desktop](https://www.docker.com/products/docker-desktop) 和 [.NET 10 SDK](https://dotnet.microsoft.com/)。
2. **运行项目**:

   ```bash
   dotnet run --project src/RustFS.Demo.AppHost/RustFS.Demo.AppHost.csproj
   ```

3. **访问 Dashboard**: 启动后，打开控制台输出的 Dashboard 链接（通常为 `http://localhost:15000`）即可管理和监控服务。

## 🧪 运行测试

运行以下命令以执行测试套件：

```bash
dotnet test
```
