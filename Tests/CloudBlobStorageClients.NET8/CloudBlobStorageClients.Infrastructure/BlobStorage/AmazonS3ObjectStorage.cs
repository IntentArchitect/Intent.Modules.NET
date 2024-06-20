using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using CloudBlobStorageClients.Application.Common.Storage;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AmazonS3.ObjectStorage.AmazonS3ObjectStorageImplementation", Version = "1.0")]

namespace CloudBlobStorageClients.Infrastructure.BlobStorage;

public class AmazonS3ObjectStorage : IObjectStorage
{
    private readonly IAmazonS3 _client;
    private readonly IConfiguration _configuration;

    public AmazonS3ObjectStorage(IAmazonS3 client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }

    public async Task<Uri> GetAsync(string bucketName, string key, CancellationToken cancellationToken = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Expires = DateTime.Now.Add(_configuration.GetValue<TimeSpan?>("AWS:PreSignedUrlExpiry") ?? TimeSpan.FromMinutes(5))
        };

        var url = await _client.GetPreSignedURLAsync(request).ConfigureAwait(false);
        return new Uri(url);
    }

    public async IAsyncEnumerable<Uri> ListAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        var request = new ListObjectsV2Request
        {
            BucketName = bucketName
        };

        var response = await _client.ListObjectsV2Async(request, cancellationToken).ConfigureAwait(false);
        foreach (var s3Object in response.S3Objects)
        {
            yield return await GetAsync(bucketName, s3Object.Key, cancellationToken);
        }
    }

    public Task<Uri> UploadAsync(Uri cloudStorageLocation, Stream dataStream, CancellationToken cancellationToken = default)
    {
        var s3Uri = new AmazonS3Uri(cloudStorageLocation);
        return UploadAsync(s3Uri.Bucket, s3Uri.Key, dataStream, cancellationToken);
    }

    public async Task<Uri> UploadAsync(string bucketName, string key, Stream dataStream, CancellationToken cancellationToken = default)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = dataStream
        };

        await _client.PutObjectAsync(putRequest, cancellationToken).ConfigureAwait(false);
        return await GetAsync(bucketName, key, cancellationToken);
    }

    public async IAsyncEnumerable<Uri> BulkUploadAsync(string bucketName, IEnumerable<BulkObjectItem> objects, CancellationToken cancellationToken = default)
    {
        foreach (var blob in objects)
        {
            yield return await UploadAsync(bucketName, blob.Name, blob.DataStream, cancellationToken);
        }
    }

    public Task<Stream> DownloadAsync(Uri cloudStorageLocation, CancellationToken cancellationToken = default)
    {
        var s3Uri = new AmazonS3Uri(cloudStorageLocation);
        return DownloadAsync(s3Uri.Bucket, s3Uri.Key, cancellationToken);
    }

    public async Task<Stream> DownloadAsync(string bucketName, string key, CancellationToken cancellationToken = default)
    {
        var getRequest = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        var response = await _client.GetObjectAsync(getRequest, cancellationToken).ConfigureAwait(false);
        return response.ResponseStream;
    }

    public Task DeleteAsync(Uri cloudStorageLocation, CancellationToken cancellationToken = default)
    {
        var s3Uri = new AmazonS3Uri(cloudStorageLocation);
        return DeleteAsync(s3Uri.Bucket, s3Uri.Key, cancellationToken);
    }

    public async Task DeleteAsync(string bucketName, string key, CancellationToken cancellationToken = default)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        await _client.DeleteObjectAsync(deleteRequest, cancellationToken).ConfigureAwait(false);
    }
}