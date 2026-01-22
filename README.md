# RustFS.Demo

[![CI](https://github.com/rustfs/rustfs-dotnet-demo/actions/workflows/ci.yml/badge.svg)](https://github.com/rustfs/rustfs-dotnet-demo/actions/workflows/ci.yml)

English | [中文](./README_zh.md)

A demo project based on **.NET Aspire**, showcasing how to orchestrate **RustFS** (High-performance S3-compatible object storage) and **ASP.NET Core Web API**.

## 🚀 Tech Stack

- **.NET 10**
- **.NET Aspire**
- **RustFS** (S3-compatible object storage)
- **Docker**

## 🛠️ Quick Start

1. **Prerequisites**: Ensure you have [Docker Desktop](https://www.docker.com/products/docker-desktop) and [.NET 10 SDK](https://dotnet.microsoft.com/) installed.
2. **Run the Project**:

   ```bash
   dotnet run --project src/RustFS.Demo.AppHost/RustFS.Demo.AppHost.csproj
   ```

3. **Access Dashboard**: After startup, open the Dashboard link output in the console (usually `http://localhost:15000`) to manage and monitor services.

## 🧪 Running Tests

Run the following command to execute the test suite:

```bash
dotnet test
```
