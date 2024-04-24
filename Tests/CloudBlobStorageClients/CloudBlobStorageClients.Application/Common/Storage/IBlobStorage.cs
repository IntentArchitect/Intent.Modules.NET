using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.BlobStorage.BlobStorageInterface", Version = "1.0")]

namespace CloudBlobStorageClients.Application.Common.Storage
{
    /// <summary>
    /// Represents a single item used for bulk uploads to blob storage.
    /// </summary>
    public record BulkBlobItem(string Name, Stream DataStream);

    /// <summary>
    /// A simplified service interface to access Blob Storage.
    /// </summary>
    public interface IBlobStorage
    {
        /// <summary>
        /// Retrieves the URI of a specific blob from a given container.
        /// </summary>
        /// <param name="containerName">The name of the blob container.</param>
        /// <param name="blobName">The name of the blob.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>The URI of the blob.</returns>
        Task<Uri> GetAsync(string containerName, string blobName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists the URIs of all blobs in a given container.
        /// </summary>
        /// <param name="containerName">The name of the blob container.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>An async enumerable of blob URIs.</returns>
        IAsyncEnumerable<Uri> ListAsync(string containerName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uploads data to a specific location in blob storage.
        /// </summary>
        /// <param name="cloudStorageLocation">The URI specifying where to upload the data.</param>
        /// <param name="dataStream">The stream of data to upload.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>The URI of the uploaded blob.</returns>
        Task<Uri> UploadAsync(Uri cloudStorageLocation, Stream dataStream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uploads data to a specific blob in a given container.
        /// </summary>
        /// <param name="containerName">The name of the blob container.</param>
        /// <param name="blobName">The name of the blob.</param>
        /// <param name="dataStream">The stream of data to upload.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>The URI of the uploaded blob.</returns>
        Task<Uri> UploadAsync(string containerName, string blobName, Stream dataStream, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs bulk upload of multiple blobs to a specific container.
        /// </summary>
        /// <param name="containerName">The name of the blob container.</param>
        /// <param name="blobs">The enumerable of bulk blob items to upload.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>An async enumerable of blob URIs for each uploaded blob.</returns>
        IAsyncEnumerable<Uri> BulkUploadAsync(string containerName, IEnumerable<BulkBlobItem> blobs, CancellationToken cancellationToken = default);

        /// <summary>
        /// Downloads data from a specific location in blob storage.
        /// </summary>
        /// <param name="cloudStorageLocation">The URI specifying where to download the data from.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>A stream containing the downloaded data.</returns>
        Task<Stream> DownloadAsync(Uri cloudStorageLocation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Downloads data from a specific blob in a given container.
        /// </summary>
        /// <param name="containerName">The name of the blob container.</param>
        /// <param name="blobName">The name of the blob.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>A stream containing the downloaded data.</returns>
        Task<Stream> DownloadAsync(string containerName, string blobName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a blob at a specific location in blob storage.
        /// </summary>
        /// <param name="cloudStorageLocation">The URI specifying the blob to delete.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        Task DeleteAsync(Uri cloudStorageLocation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a specific blob in a given container.
        /// </summary>
        /// <param name="containerName">The name of the blob container.</param>
        /// <param name="blobName">The name of the blob.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        Task DeleteAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    }
}