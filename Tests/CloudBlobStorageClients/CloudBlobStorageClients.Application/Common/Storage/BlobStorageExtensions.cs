using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.BlobStorage.BlobStorageExtensions", Version = "1.0")]

namespace CloudBlobStorageClients.Application.Common.Storage
{
    /// <summary>
    /// Contains extension methods for the <see cref="IBlobStorage"/> interface.
    /// </summary>
    public static class BlobStorageExtensions
    {
        /// <summary>
        /// Uploads a string content to a specified cloud storage location.
        /// </summary>
        /// <param name="storage">The blob storage instance to which the string will be uploaded.</param>
        /// <param name="cloudStorageLocation">The URI specifying where to upload the string.</param>
        /// <param name="stringContent">The string content to be uploaded.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>The URI of the uploaded blob.</returns>
        public static Task<Uri> UploadStringAsync(this IBlobStorage storage, Uri cloudStorageLocation, string stringContent, CancellationToken cancellationToken = default)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(stringContent));
            return storage.UploadAsync(cloudStorageLocation, stream, cancellationToken);
        }

        /// <summary>
        /// Uploads a string content to a specific blob in a given container.
        /// </summary>
        /// <param name="storage">The blob storage instance to which the string will be uploaded.</param>
        /// <param name="containerName">The name of the blob container.</param>
        /// <param name="blobName">The name of the blob.</param>
        /// <param name="stringContent">The string content to be uploaded.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>The URI of the uploaded blob.</returns>
        public static Task<Uri> UploadStringAsync(this IBlobStorage storage, string containerName, string blobName, string stringContent, CancellationToken cancellationToken = default)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(stringContent));
            return storage.UploadAsync(containerName, blobName, stream, cancellationToken);
        }

        /// <summary>
        /// Downloads the content of a blob from a specified cloud storage location as a string.
        /// </summary>
        /// <param name="storage">The blob storage instance from which the string will be downloaded.</param>
        /// <param name="cloudStorageLocation">The URI specifying the blob to be downloaded.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>The downloaded string content.</returns>
        public static async Task<string> DownloadAsStringAsync(this IBlobStorage storage, Uri cloudStorageLocation, CancellationToken cancellationToken = default)
        {
            var result = await storage.DownloadAsync(cloudStorageLocation, cancellationToken).ConfigureAwait(false);
            var text = await new StreamReader(result).ReadToEndAsync(cancellationToken).ConfigureAwait(false);
            return text;
        }

        /// <summary>
        /// Downloads the content of a specific blob in a given container as a string.
        /// </summary>
        /// <param name="storage">The blob storage instance from which the string will be downloaded.</param>
        /// <param name="containerName">The name of the blob container.</param>
        /// <param name="blobName">The name of the blob.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>The downloaded string content.</returns>
        public static async Task<string> DownloadAsStringAsync(this IBlobStorage storage, string containerName, string blobName, CancellationToken cancellationToken = default)
        {
            var result = await storage.DownloadAsync(containerName, blobName, cancellationToken).ConfigureAwait(false);
            var text = await new StreamReader(result).ReadToEndAsync(cancellationToken).ConfigureAwait(false);
            return text;
        }
    }
}