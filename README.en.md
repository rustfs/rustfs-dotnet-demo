

# RustFS .NET S3 Demo Project

This is a .NET S3 demo project implemented using RustFS technology, demonstrating how to interact with S3-compatible storage services in a .NET environment.

## Feature Overview

This project mainly implements the following functions:
- Bucket Management: Create, delete, check existence, and list all buckets
- File Management: Upload, download, delete, and list files within a bucket
- Unified API response format
- Exception handling middleware

## Key Features

- Perform object storage operations using AWS S3-compatible APIs
- Clear error handling and response format
- Modular code structure
- Support for asynchronous operations

## API Specifications

All APIs follow a unified response format containing the following fields:
- Success: Whether the operation was successful
- Code: HTTP status code
- Message: Description of the operation result
- Data: Returned data (if any)

## Usage Examples

### Create a Bucket
```http
POST /api/bucket/{bucketName}
```

### Upload a File
```http
POST /api/file/upload/{bucketName}/{key}
Form-data: file=@local_file_path
```

### Download a File
```http
GET /api/file/download/{bucketName}/{key}
```

### Delete a File
```http
DELETE /api/file/{bucketName}/{key}
```

## Project Structure

- Controllers: Contains controller classes such as BucketManageController.cs and FileManageController.cs
- Extensions: Contains controller extension methods
- Handlers: Contains classes related to exception handling
- Models: Contains response model classes
- Services: Contains AWS S3 service implementation
- Settings: Contains AWS S3 configuration settings

## Error Handling

The project uses a unified exception handling mechanism. All exceptions will be converted into ProblemDetailsResponse format before being returned to the client.

## Configuration

AWS S3 connection information is configured through the AwsS3Setting class, which includes service URL, access key, secret key, and region settings.

## License

This project is licensed under the Apache-2.0 License.