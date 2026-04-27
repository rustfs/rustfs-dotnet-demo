namespace RustFS.Demo.Web.Services;

public static partial class RustFSServiceLogExtensions
{
    // Bucket Operations (1000 - 1099)
    [LoggerMessage(1001, LogLevel.Information, "Starting to create bucket: {BucketName}")]
    public static partial void LogStartingCreateBucket(this ILogger logger, string bucketName);

    [LoggerMessage(1002, LogLevel.Information, "Bucket already exists: {BucketName}")]
    public static partial void LogBucketAlreadyExists(this ILogger logger, string bucketName);

    [LoggerMessage(1003, LogLevel.Information, "Bucket created successfully: {BucketName}")]
    public static partial void LogBucketCreated(this ILogger logger, string bucketName);

    [LoggerMessage(1004, LogLevel.Debug, "Checking if bucket exists: {BucketName} -> {Exists}")]
    public static partial void LogCheckingBucketExists(this ILogger logger, string bucketName, bool exists);

    [LoggerMessage(1005, LogLevel.Information, "Starting to delete bucket: {BucketName}")]
    public static partial void LogStartingDeleteBucket(this ILogger logger, string bucketName);

    [LoggerMessage(1006, LogLevel.Information, "Bucket deleted successfully: {BucketName}")]
    public static partial void LogBucketDeleted(this ILogger logger, string bucketName);

    [LoggerMessage(1007, LogLevel.Information, "Retrieved {Count} buckets")]
    public static partial void LogBucketsRetrieved(this ILogger logger, int count);

    [LoggerMessage(1008, LogLevel.Information, "Bucket {BucketName} does not exist, creating it")]
    public static partial void LogBucketNotExistCreating(this ILogger logger, string bucketName);

    [LoggerMessage(1009, LogLevel.Debug, "Listing files in bucket: {BucketName}")]
    public static partial void LogListingFiles(this ILogger logger, string bucketName);

    [LoggerMessage(1010, LogLevel.Warning, "List files failed, bucket does not exist: {BucketName}")]
    public static partial void LogListFilesBucketNotExist(this ILogger logger, string bucketName);

    [LoggerMessage(1011, LogLevel.Information, "Bucket {BucketName} contains {Count} files")]
    public static partial void LogBucketFileCount(this ILogger logger, string bucketName, int count);


    // File Operations (1100 - 1199)
    [LoggerMessage(1100, LogLevel.Information, "Generating presigned URL: {BucketName}/{Key}")]
    public static partial void LogGeneratingPresignedUrl(this ILogger logger, string? bucketName, string? key);

    [LoggerMessage(1101, LogLevel.Debug, "Presigned URL generated successfully: {Url}")]
    public static partial void LogPresignedUrlGenerated(this ILogger logger, string url);

    [LoggerMessage(1102, LogLevel.Information, "Starting file upload: {BucketName}/{Key}, Size: {Size} bytes")]
    public static partial void LogStartingFileUpload(this ILogger logger, string bucketName, string key, long size);

    [LoggerMessage(1103, LogLevel.Information, "File uploaded successfully: {BucketName}/{Key}")]
    public static partial void LogFileUploaded(this ILogger logger, string bucketName, string key);

    [LoggerMessage(1104, LogLevel.Information, "Getting file: {BucketName}/{Key}")]
    public static partial void LogGettingFile(this ILogger logger, string bucketName, string key);

    [LoggerMessage(1105, LogLevel.Information, "File stream retrieved successfully: {BucketName}/{Key}, Size: {Size} bytes")]
    public static partial void LogFileRetrieved(this ILogger logger, string bucketName, string key, long size);

    [LoggerMessage(1106, LogLevel.Information, "Deleting file: {BucketName}/{Key}")]
    public static partial void LogDeletingFile(this ILogger logger, string bucketName, string key);

    [LoggerMessage(1107, LogLevel.Information, "File deleted successfully: {BucketName}/{Key}")]
    public static partial void LogFileDeleted(this ILogger logger, string bucketName, string key);

    [LoggerMessage(1108, LogLevel.Warning, "File deletion might have failed for {BucketName}, status code: {StatusCode}")]
    public static partial void LogFileDeletionFailed(this ILogger logger, string bucketName, System.Net.HttpStatusCode statusCode);
}
