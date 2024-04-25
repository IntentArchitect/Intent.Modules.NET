using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AmazonS3.ObjectStorage.ObjectStorageInterface", Version = "1.0")]

namespace CloudBlobStorageClients.Application.Common.Storage;

/// <summary>
/// Represents a single item used for bulk uploads to object storage.
/// </summary>
public record BulkObjectItem(string Name, Stream DataStream);

/// <summary>
/// A simplified service interface to access Object Storage.
/// </summary>
public interface IObjectStorage
{
    /// <summary>
    /// Retrieves the URI of a specific object from a given bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="key">The key name of the object.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
    /// <returns>The URI of the object.</returns>
    Task<Uri> GetAsync(string bucketName, string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the URIs of all objects in a given bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
    /// <returns>An async enumerable of object URIs.</returns>
    IAsyncEnumerable<Uri> ListAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads data to a specific location in object storage.
    /// </summary>
    /// <param name="cloudStorageLocation">The URI specifying where to upload the data.</param>
    /// <param name="dataStream">The stream of data to upload.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
    /// <returns>The URI of the uploaded object.</returns>
    Task<Uri> UploadAsync(Uri cloudStorageLocation, Stream dataStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads data to a specific object in a given bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="key">The key name of the object.</param>
    /// <param name="dataStream">The stream of data to upload.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
    /// <returns>The URI of the uploaded object.</returns>
    Task<Uri> UploadAsync(string bucketName, string key, Stream dataStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs bulk upload of multiple objects to a specific bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objects">The enumerable of bulk object items to upload.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
    /// <returns>An async enumerable of object URIs for each uploaded object.</returns>
    IAsyncEnumerable<Uri> BulkUploadAsync(string bucketName, IEnumerable<BulkObjectItem> objects, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads data from a specific location in object storage.
    /// </summary>
    /// <param name="cloudStorageLocation">The URI specifying where to download the data from.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
    /// <returns>A stream containing the downloaded data.</returns>
    Task<Stream> DownloadAsync(Uri cloudStorageLocation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads data from a specific object in a given bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="key">The key name of the object.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
    /// <returns>A stream containing the downloaded data.</returns>
    Task<Stream> DownloadAsync(string bucketName, string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a object at a specific location in object storage.
    /// </summary>
    /// <param name="cloudStorageLocation">The URI specifying the object to delete.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
    Task DeleteAsync(Uri cloudStorageLocation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific object in a given bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="key">The key name of the object.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
    Task DeleteAsync(string bucketName, string key, CancellationToken cancellationToken = default);
}