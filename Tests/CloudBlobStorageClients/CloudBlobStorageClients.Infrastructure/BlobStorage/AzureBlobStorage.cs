using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CloudBlobStorageClients.Application.Common.Storage;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.BlobStorage.AzureBlobStorageImplementation", Version = "1.0")]

namespace CloudBlobStorageClients.Infrastructure.BlobStorage
{
    public class AzureBlobStorage : IBlobStorage
    {
        private const PublicAccessType ContainerPublicAccessType = PublicAccessType.None;
        private readonly BlobServiceClient _client;

        public AzureBlobStorage(IConfiguration configuration)
        {
            _client = new BlobServiceClient(configuration.GetValue<string>("AzureBlobStorage"));
        }

        public async Task<Uri> GetAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
        {
            var blobClient = await GetBlobClient(containerName, blobName, cancellationToken).ConfigureAwait(false);
            return blobClient.Uri;
        }

        public async IAsyncEnumerable<Uri> ListAsync(string containerName,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var containerClient = await GetContainerClientAsync(containerName, cancellationToken).ConfigureAwait(false);
            await foreach (var blobItem in containerClient.GetBlobsAsync(cancellationToken: cancellationToken))
            {
                yield return await GetAsync(containerName, blobItem.Name, cancellationToken);
            }
        }

        public Task<Uri> UploadAsync(Uri cloudStorageLocation, Stream dataStream, CancellationToken cancellationToken = default)
        {
            var blobUriBuilder = new BlobUriBuilder(cloudStorageLocation);
            return UploadAsync(blobUriBuilder.BlobContainerName, blobUriBuilder.BlobName, dataStream, cancellationToken);
        }

        public async Task<Uri> UploadAsync(string containerName, string blobName, Stream dataStream,
            CancellationToken cancellationToken = default)
        {
            var blobClient = await GetBlobClient(containerName, blobName, cancellationToken).ConfigureAwait(false);
            await blobClient.UploadAsync(dataStream, overwrite: true, cancellationToken).ConfigureAwait(false);
            return blobClient.Uri;
        }

        public async IAsyncEnumerable<Uri> BulkUploadAsync(
            string containerName,
            IEnumerable<BulkBlobItem> blobs,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var blob in blobs)
            {
                yield return await UploadAsync(containerName, blob.Name, blob.DataStream, cancellationToken);
            }
        }

        public Task<Stream> DownloadAsync(Uri cloudStorageLocation, CancellationToken cancellationToken = default)
        {
            var blobUriBuilder = new BlobUriBuilder(cloudStorageLocation);
            return DownloadAsync(blobUriBuilder.BlobContainerName, blobUriBuilder.BlobName, cancellationToken);
        }

        public async Task<Stream> DownloadAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
        {
            var blobClient = await GetBlobClient(containerName, blobName, cancellationToken).ConfigureAwait(false);
            var result = await blobClient.DownloadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Value.Content;
        }

        public async Task DeleteAsync(Uri cloudStorageLocation, CancellationToken cancellationToken = default)
        {
            var blobUriBuilder = new BlobUriBuilder(cloudStorageLocation);
            await DeleteAsync(blobUriBuilder.BlobContainerName, blobUriBuilder.BlobName, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
        {
            var blobClient = await GetBlobClient(containerName, blobName, cancellationToken);
            await blobClient.DeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private async Task<BlobContainerClient> GetContainerClientAsync(string containerName, CancellationToken cancellationToken)
        {
            var containerClient = _client.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(ContainerPublicAccessType, cancellationToken: cancellationToken);
            return containerClient;
        }

        private async Task<BlobClient> GetBlobClient(string containerName, string blobName, CancellationToken cancellationToken)
        {
            var containerClient = await GetContainerClientAsync(containerName, cancellationToken: cancellationToken);
            return containerClient.GetBlobClient(blobName);
        }
    }
}