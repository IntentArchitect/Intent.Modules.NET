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
        private readonly IAzureBlobStorage _aws;
        private readonly ILogger<TestAzureCommandHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public TestAzureCommandHandler(IAzureBlobStorage aws, ILogger<TestAzureCommandHandler> logger)
        {
            _aws = aws;
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(TestAzureCommand request, CancellationToken cancellationToken)
        {
            const string containerName = "dan-test-bucket";
            
            var url = await _aws.UploadStringAsync(containerName, "test-file", "This is a test string", cancellationToken);
            _logger.LogInformation("UPLOAD: {Ur}", url);

            var url2 = await _aws.GetAsync(containerName, "test-file", cancellationToken);
            _logger.LogInformation("GET: {Ur}", url2);

            await foreach (var item in _aws.ListAsync(containerName, cancellationToken))
            {
                _logger.LogInformation("LIST: {Url}", item);
                var content = await _aws.DownloadAsStringAsync(item, cancellationToken);
                _logger.LogInformation("CONTENT: {Content}", content);
            }

            await _aws.DeleteAsync(containerName, "test-file", cancellationToken);
        }
    }
}