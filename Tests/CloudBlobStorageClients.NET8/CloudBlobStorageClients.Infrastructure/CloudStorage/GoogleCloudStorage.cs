using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CloudBlobStorageClients.Application.Common.Storage;
using Google.Cloud.Storage.V1;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Google.CloudStorage.GoogleCloudStorageImplementation", Version = "1.0")]

namespace CloudBlobStorageClients.Infrastructure.CloudStorage
{
    public class GoogleCloudStorage : ICloudStorage
    {
        private readonly StorageClient _client;
        private readonly IConfiguration _configuration;

        public GoogleCloudStorage(StorageClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task<Uri> GetAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
        {
            var urlSigner = _client.CreateUrlSigner();
            var url = await urlSigner.SignAsync(bucketName, objectName, _configuration.GetValue<TimeSpan?>("GCP:PreSignedUrlExpiry") ?? TimeSpan.FromMinutes(5), cancellationToken: cancellationToken).ConfigureAwait(false);
            return new Uri(url);
        }

        public async IAsyncEnumerable<Uri> ListAsync(
            string bucketName,
            string? prefix = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var objects = _client.ListObjectsAsync(bucketName, prefix).AsRawResponses().ConfigureAwait(false);

            await foreach (var @object in objects)
            {
                foreach (var gcObject in @object.Items)
                {
                    yield return await GetAsync(bucketName, gcObject.Name, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task<Stream> DownloadAsync(
            string bucketName,
            string objectName,
            CancellationToken cancellationToken = default)
        {
            var returnStream = new MemoryStream();
            _ = await _client.DownloadObjectAsync(bucketName, objectName, returnStream, cancellationToken: cancellationToken).ConfigureAwait(false);
            returnStream.Position = 0;
            return returnStream;
        }

        public async Task<Uri> UploadAsync(
            string bucketName,
            string objectName,
            Stream dataStream,
            string? contentType = null,
            CancellationToken cancellationToken = default)
        {
            _ = await _client.UploadObjectAsync(bucketName, objectName, contentType, dataStream, cancellationToken: cancellationToken).ConfigureAwait(false);
            return await GetAsync(bucketName, objectName, cancellationToken).ConfigureAwait(false);
        }

        public async IAsyncEnumerable<Uri> BulkUploadAsync(
            string bucketName,
            IEnumerable<BulkCloudObjectItem> objects,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var cloudObject in objects)
            {
                yield return await UploadAsync(bucketName, cloudObject.Name, cloudObject.DataStream, cloudObject.ContentType, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task DeleteAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
        {
            await _client.DeleteObjectAsync(bucketName, objectName, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}