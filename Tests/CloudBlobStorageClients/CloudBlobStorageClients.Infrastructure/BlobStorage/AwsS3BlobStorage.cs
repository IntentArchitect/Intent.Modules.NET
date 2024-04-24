using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using CloudBlobStorageClients.Application.Common.Storage;

namespace CloudBlobStorageClients.Infrastructure.BlobStorage;

public class AwsS3BlobStorage : IAwsS3BlobStorage
{
    private readonly IAmazonS3 _client;

    public AwsS3BlobStorage(IAmazonS3 client)
    {
        _client = client;
    }

    public async Task<Uri> GetAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = containerName,
            Key = blobName,
            Expires = DateTime.Now.AddMinutes(5) // Adjust expiration as needed
        };

        var url = await _client.GetPreSignedURLAsync(request).ConfigureAwait(false);
        return new Uri(url);
    }

    public async IAsyncEnumerable<Uri> ListAsync(string containerName, CancellationToken cancellationToken = default)
    {
        var request = new ListObjectsV2Request
        {
            BucketName = containerName
        };

        var response = await _client.ListObjectsV2Async(request, cancellationToken).ConfigureAwait(false);
        foreach (var s3Object in response.S3Objects)
        {
            yield return await GetAsync(containerName, s3Object.Key, cancellationToken);
        }
    }

    public Task<Uri> UploadAsync(Uri cloudStorageLocation, Stream dataStream, CancellationToken cancellationToken = default)
    {
        var s3Uri = new AmazonS3Uri(cloudStorageLocation);
        return UploadAsync(s3Uri.Bucket, s3Uri.Key, dataStream, cancellationToken);
    }

    public async Task<Uri> UploadAsync(string containerName, string blobName, Stream dataStream, CancellationToken cancellationToken = default)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = containerName,
            Key = blobName,
            InputStream = dataStream
        };

        await _client.PutObjectAsync(putRequest, cancellationToken).ConfigureAwait(false);
        return await GetAsync(containerName, blobName, cancellationToken);
    }

    public async IAsyncEnumerable<Uri> BulkUploadAsync(string containerName, IEnumerable<BulkBlobItem> blobs, CancellationToken cancellationToken = default)
    {
        foreach (var blob in blobs)
        {
            yield return await UploadAsync(containerName, blob.Name, blob.DataStream, cancellationToken);
        }
    }

    public Task<Stream> DownloadAsync(Uri cloudStorageLocation, CancellationToken cancellationToken = default)
    {
        var s3Uri = new AmazonS3Uri(cloudStorageLocation);
        return DownloadAsync(s3Uri.Bucket, s3Uri.Key, cancellationToken);
    }

    public async Task<Stream> DownloadAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        var getRequest = new GetObjectRequest
        {
            BucketName = containerName,
            Key = blobName
        };

        var response = await _client.GetObjectAsync(getRequest, cancellationToken).ConfigureAwait(false);
        return response.ResponseStream;
    }

    public Task DeleteAsync(Uri cloudStorageLocation, CancellationToken cancellationToken = default)
    {
        var s3Uri = new AmazonS3Uri(cloudStorageLocation);
        return DeleteAsync(s3Uri.Bucket, s3Uri.Key, cancellationToken);
    }

    public async Task DeleteAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = containerName,
            Key = blobName
        };

        await _client.DeleteObjectAsync(deleteRequest, cancellationToken).ConfigureAwait(false);
    }
}