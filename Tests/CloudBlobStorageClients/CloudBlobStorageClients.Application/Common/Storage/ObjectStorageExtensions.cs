using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AmazonS3.ObjectStorage.ObjectStorageExtensions", Version = "1.0")]

namespace CloudBlobStorageClients.Application.Common.Storage
{
    /// <summary>
    /// Contains extension methods for the <see cref="IObjectStorage"/> interface.
    /// </summary>
    public static class ObjectStorageExtensions
    {
        /// <summary>
        /// Uploads a string content to a specified cloud storage location.
        /// </summary>
        /// <param name="storage">The object storage instance to which the string will be uploaded.</param>
        /// <param name="cloudStorageLocation">The URI specifying where to upload the string.</param>
        /// <param name="stringContent">The string content to be uploaded.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>The URI of the uploaded object.</returns>
        public static Task<Uri> UploadStringAsync(this IObjectStorage storage, Uri cloudStorageLocation, string stringContent, CancellationToken cancellationToken = default)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(stringContent));
            return storage.UploadAsync(cloudStorageLocation, stream, cancellationToken);
        }

        /// <summary>
        /// Uploads a string content to a specific object in a given bucket.
        /// </summary>
        /// <param name="storage">The object storage instance to which the string will be uploaded.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="key">The name of the object.</param>
        /// <param name="stringContent">The string content to be uploaded.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>The URI of the uploaded object.</returns>
        public static Task<Uri> UploadStringAsync(this IObjectStorage storage, string bucketName, string key, string stringContent, CancellationToken cancellationToken = default)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(stringContent));
            return storage.UploadAsync(bucketName, key, stream, cancellationToken);
        }

        /// <summary>
        /// Downloads the content of a object from a specified cloud storage location as a string.
        /// </summary>
        /// <param name="storage">The object storage instance from which the string will be downloaded.</param>
        /// <param name="cloudStorageLocation">The URI specifying the object to be downloaded.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>The downloaded string content.</returns>
        public static async Task<string> DownloadAsStringAsync(this IObjectStorage storage, Uri cloudStorageLocation, CancellationToken cancellationToken = default)
        {
            var result = await storage.DownloadAsync(cloudStorageLocation, cancellationToken).ConfigureAwait(false);
            var text = await new StreamReader(result).ReadToEndAsync(cancellationToken).ConfigureAwait(false);
            return text;
        }

        /// <summary>
        /// Downloads the content of a specific object in a given container as a string.
        /// </summary>
        /// <param name="storage">The object storage instance from which the string will be downloaded.</param>
        /// <param name="bucketName">The name of the bucket.</param>
        /// <param name="key">The name of the object.</param>
        /// <param name="cancellationToken">An optional token to cancel the asynchronous operation.</param>
        /// <returns>The downloaded string content.</returns>
        public static async Task<string> DownloadAsStringAsync(this IObjectStorage storage, string bucketName, string key, CancellationToken cancellationToken = default)
        {
            var result = await storage.DownloadAsync(bucketName, key, cancellationToken).ConfigureAwait(false);
            var text = await new StreamReader(result).ReadToEndAsync(cancellationToken).ConfigureAwait(false);
            return text;
        }
    }
}