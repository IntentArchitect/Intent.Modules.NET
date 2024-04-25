using System;
using System.Threading;
using System.Threading.Tasks;
using CloudBlobStorageClients.Application.Common.Storage;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CloudBlobStorageClients.Application.Tests.TestAzure
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestAzureCommandHandler : IRequestHandler<TestAzureCommand>
    {
        private readonly IBlobStorage _blobStorage;
        private readonly ILogger<TestAzureCommandHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public TestAzureCommandHandler(IBlobStorage blobStorage, ILogger<TestAzureCommandHandler> logger)
        {
            _blobStorage = blobStorage;
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(TestAzureCommand request, CancellationToken cancellationToken)
        {
            const string containerName = "dan-test-bucket";

            Uri url;

            url = await _blobStorage.UploadStringAsync(containerName, "test-file", "This is a test string", cancellationToken);
            _logger.LogInformation("UPLOAD: {Ur}", url);
            url = await _blobStorage.UploadStringAsync(url, "This is a test string 2", cancellationToken);
            _logger.LogInformation("UPLOAD: {Ur}", url);

            url = await _blobStorage.GetAsync(containerName, "test-file", cancellationToken);
            _logger.LogInformation("GET: {Ur}", url);

            await foreach (var item in _blobStorage.ListAsync(containerName, cancellationToken))
            {
                _logger.LogInformation("LIST: {Url}", item);
                var content = await _blobStorage.DownloadAsStringAsync(item, cancellationToken);
                _logger.LogInformation("CONTENT: {Content}", content);
            }

            await _blobStorage.DeleteAsync(containerName, "test-file", cancellationToken);
        }
    }
}