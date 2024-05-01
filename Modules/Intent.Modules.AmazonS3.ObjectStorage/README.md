# Amazon S3 Object Storage

The Amazon S3 Object Storage module simplifies working with Amazon S3 in .NET applications by providing a robust C# client wrapper. This module includes an integration of the `AWSSDK.S3` NuGet package, facilitating direct interactions with Amazon S3 services.

## Module Overview

Amazon S3 is designed to store massive amounts of unstructured data, such as files and media. The Amazon S3 Object Storage Module abstracts these interactions through the `IObjectStorage` interface, which defines methods for essential operations like upload, download, list, and delete. This interface ensures that the implementation details are encapsulated, allowing developers to focus on business logic rather than infrastructure management.

## Practical Example

Consider a scenario within a corporate software system where it is necessary to store and fetch marketing materials from Amazon S3. Below is an illustration of how the `IObjectStorage` interface can be implemented in a service class to manage these operations:

```csharp
public class MarketingMaterialService
{
    private readonly IObjectStorage _objectStorage;
    private readonly string _bucketName = "marketing-materials";

    public MarketingMaterialService(IObjectStorage objectStorage)
    {
        _objectStorage = objectStorage;
    }

    public async Task<Uri> SaveMaterialAsync(string materialName, string content)
    {
        var materialUri = await _objectStorage.UploadStringAsync(_bucketName, materialName, content);
        return materialUri;
    }

    public async Task<string> FetchMaterialAsync(string materialName)
    {
        var materialContent = await _objectStorage.DownloadAsStringAsync(_bucketName, materialName);
        return materialContent;
    }

    public async Task<IEnumerable<Uri>> ListMaterialsAsync()
    {
        var materials = new List<Uri>();
        await foreach (var uri in _objectStorage.ListAsync(_bucketName))
        {
            materials.Add(uri);
        }
        return materials;
    }
}
```

This service class simplifies all functions related to storing and retrieving marketing materials, enhancing the manageability and cleanliness of the overall codebase.

## Pre-Signed Expiry Urls

Performing a `GetAsync` will give you back a link that you can use to gain access to an object for a limited amount of time ([link](https://docs.aws.amazon.com/AmazonS3/latest/userguide/using-presigned-url.html)).

To configure this expiry time you can add this entry in your `appsettings.json` file and specify the expiry duration as a TimeSpan.

```json
"AWS": {
  "PreSignedUrlExpiry": "00:00:15" // Expire 15 seconds from now
}
```

## Local Configuration

For local development, developers can simulate an Amazon S3 environment using an emulator, avoiding the need for real S3 buckets. One useful tool for this purpose is `S3 Ninja`, which can be set up using Docker. Here are the steps to configure and use `S3 Ninja` for local development:

1. Pull the `S3 Ninja` Docker image using the command:
   ```
   docker pull scireum/s3-ninja:8.3.2
   ```
2. Run the `S3 Ninja` container:
   ```
   docker run -p 9000:9000 scireum/s3-ninja:8.3.2
   ```
3. Open your web browser and navigate to `http://localhost:9000/ui` to verify the application is running. This interface also allows you to obtain the Access and Secret Keys.
4. To integrate with `S3 Ninja`, set up a profile with the necessary credentials:
    - For developers with the [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html#getting-started-install-instructions) installed, execute the following commands:
      ```
      aws configure set profile.s3ninja.aws_access_key_id {your s3 ninja access key}
      aws configure set profile.s3ninja.aws_secret_access_key {your s3 ninja secret key}
      ```
    - Alternatively, add the credentials directly to the AWS credentials file located at `C:\Users\<userid>\.aws\credentials`. Update the file as follows:
      ```ini
      [s3ninja]
      aws_access_key_id = {your s3 ninja access key}
      aws_secret_access_key = {your s3 ninja secret key}
      ```
5. Update your `appsettings.json` to include the following configuration:
   ```json
   "AWS": {
    "Profile": "s3ninja",
    "ServiceURL": "http://localhost:9000",
    "ForcePathStyle": true
   }
   ```
> **Note:**
>
> Ensure to create the necessary buckets using the S3 Ninja UI prior to performing any operations with them.

> **Important:**
>
> Due to limitations in the S3 Ninja emulator, the `ForcePathStyle` setting must be enabled to ensure proper API access.
> This does mean that using operations on the Amazon S3 Object Store that requires a `URI` parameter will not work. This is due to the fact that the different path style cannot be parsed by the S3 client in order to know what the bucket name and object key is. The methods that require `bucket name` and `key` parameters work as per normal.

## Production Configuration

In a production environment, configuration settings for the Amazon S3 Object Storage typically come directly from the AWS Management Console. Here's the process:

1. Log into the AWS Management Console and navigate to your S3 instance.
2. Enter the "Security credentials" section.
3. Create an Access Key by using an existing or creating new a user and going to the `Security credentials` section.

The AWS SDK for .NET can automatically retrieve settings from environment variables, or directly through settings provided in `appsettings.json`.

### Environment variables

1. **AWS_ACCESS_KEY_ID** - This environment variable is used to set the AWS access key ID that is part of your AWS credentials.

2. **AWS_SECRET_ACCESS_KEY** - This corresponds to the secret part of your Amazon Web Services credentials. It is used in conjunction with the AWS access key ID to authenticate your application's requests to AWS.

3. **AWS_SESSION_TOKEN** (if applicable) - This is necessary if you are using temporary credentials that you might obtain from AWS Identity and Access Management (IAM) roles or from using AWS Security Token Service.

4. **AWS_REGION** - Specifies the geographic region where the AWS servers handling your requests are located. For instance, `eu-north-1`, `us-west-2`, etc.

5. **AWS_PROFILE** - If you utilize named profiles as part of the AWS credentials file, setting this variable will tell the SDK which profile to use.

### appsettings.json

```json
"AWS": {
    "Profile": "default",
    "Region": "eu-north-1"
}
```

Ensure to use correct security practices for managing credentials and sensitive data, especially in production environments. The above-mentioned configuration should replace any instance-specific settings in `appsettings.json` for your production deployments.