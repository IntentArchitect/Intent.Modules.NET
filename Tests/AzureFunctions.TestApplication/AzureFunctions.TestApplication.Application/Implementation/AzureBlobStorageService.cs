using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Common.Storage;
using AzureFunctions.TestApplication.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly IBlobStorage _blobStorage;

        [IntentManaged(Mode.Merge)]
        public AzureBlobStorageService(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task TestOperation(CancellationToken cancellationToken = default)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("THIS IS SPARTA!"));
            await _blobStorage.UploadAsync("test", "testfile.txt", stream, cancellationToken: cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}