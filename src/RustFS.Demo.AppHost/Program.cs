Console.WriteLine("""
  _   _      _ _          ____            _   _____ ____  
 | | | | ___| | | ___    |  _ \ _   _ ___| |_|  ___/ ___| 
 | |_| |/ _ \ | |/ _ \   | |_) | | | / __| __| |_  \___ \ 
 |  _  |  __/ | | (_) |  |  _ <| |_| \__ \ |_|  _|  ___) |
 |_| |_|\___|_|_|\___/   |_| \_\\__,_|___/\__|_|   |____/ 
                                                          
""");

var builder = DistributedApplication.CreateBuilder(args);

// 添加 RustFS 容器
var rustfs = builder.AddContainer("rustfs", "rustfs/rustfs", "latest")
    // S3 API 端口
    .WithHttpEndpoint(targetPort: 9000, name: "s3-api")
    // 控制台端口
    .WithHttpEndpoint(targetPort: 9001, name: "console")
    // 环境变量配置
    .WithEnvironment("RUSTFS_VOLUMES", "/data/rustfs0")
    .WithEnvironment("RUSTFS_ADDRESS", "0.0.0.0:9000")
    .WithEnvironment("RUSTFS_CONSOLE_ADDRESS", "0.0.0.0:9001")
    .WithEnvironment("RUSTFS_CONSOLE_ENABLE", "true")
    .WithEnvironment("RUSTFS_CORS_ALLOWED_ORIGINS", "*")
    .WithEnvironment("RUSTFS_CONSOLE_CORS_ALLOWED_ORIGINS", "*")
    .WithEnvironment("RUSTFS_ACCESS_KEY", "admin")
    .WithEnvironment("RUSTFS_SECRET_KEY", "admin123")
    .WithEnvironment("RUSTFS_OBS_LOGGER_LEVEL", "info")
    // 挂载数据卷
    .WithBindMount("./deploy/data/pro", "/data")
    .WithBindMount("./deploy/logs", "/app/logs");

_ = builder.AddProject("web", "../RustFS.Demo.Web/RustFS.Demo.Web.csproj")
    .WithReference(rustfs.GetEndpoint("s3-api"))
    .WaitFor(rustfs);

await builder.Build().RunAsync();

