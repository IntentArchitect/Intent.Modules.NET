using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Google.CloudStorage.CloudStorageInterface", Version = "1.0")]

namespace CloudBlobStorageClients.Application.Common.Storage
{
    /// <summary>
    /// A simplified service interface to access Object Storage.
    /// </summary>
    public interface ICloudStorage
    {
        /// <summary>
        /// Retrieves the URI of a specific object from a given bucket.
        /// </summary>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="objectName">The name of the object.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>The URI of the object.</returns>
        Task<Uri> GetAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
        /// <summary>
        /// Lists the URIs of all objects in a given bucket.
        /// </summary>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="prefix">The prefix to match. Only objects with names that start with this string will be returned. May be null or empty</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>An async enumerable of object URIs.</returns>
        IAsyncEnumerable<Uri> ListAsync(string bucketName, string? prefix = null, CancellationToken cancellationToken = default);
        /// <summary>
        /// Downloads data from a specific location in object storage.
        /// </summary>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="objectName">The name of the object.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>A stream containing the downloaded data.</returns>
        Task<Stream> DownloadAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
        /// <summary>
        /// Uploads data to a specific location in object storage.
        /// </summary>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="objectName">The name of the object.</param>
        /// <param name="dataStream">The stream of data to upload.</param>
        /// <param name="contentType">The content type of the object. This should be a MIME type. Can be null.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>The URI of the uploaded object.</returns>
        Task<Uri> UploadAsync(string bucketName, string objectName, Stream dataStream, string? contentType = null, CancellationToken cancellationToken = default);
        /// <summary>
        /// Performs bulk upload of multiple objects to a specific bucket.
        /// </summary>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="objects">The enumerable of bulk object items to upload.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>An async enumerable of object URIs for each uploaded object.</returns>
        IAsyncEnumerable<Uri> BulkUploadAsync(string bucketName, IEnumerable<BulkCloudObjectItem> objects, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes a specific object in a given bucket.
        /// </summary>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="objectName">The name of the object.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        Task DeleteAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
    }
}