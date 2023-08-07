using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Common.Storage;
using AzureFunctions.TestApplication.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly IBlobStorage _blobStorage;
        private readonly ILogger<AzureBlobStorageService> _logger;

        [IntentManaged(Mode.Merge)]
        public AzureBlobStorageService(IBlobStorage blobStorage, ILogger<AzureBlobStorageService> logger)
        {
            _blobStorage = blobStorage;
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task TestOperation(CancellationToken cancellationToken = default)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("SAMPLE TEXT FILE"));
            var uploadedUri = await _blobStorage.UploadAsync("test", "testfile.txt", stream, cancellationToken: cancellationToken);
            _logger.LogInformation("Uploaded URI: {UploadedUri}", uploadedUri);
            var retrievedUris = _blobStorage.ListAsync("test", cancellationToken);
            var listOfUris = new List<Uri>();
            await foreach (var currentUri in retrievedUris)
            {
                listOfUris.Add(currentUri);
            }

            var concatListUris = string.Join(", ",listOfUris);
            _logger.LogInformation("files: {ConcatListUris}", concatListUris);
        }

        public void Dispose()
        {
        }
    }
}