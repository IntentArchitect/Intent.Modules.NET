using System;
using System.Threading;
using System.Threading.Tasks;
using CloudBlobStorageClients.Application.Common.Storage;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CloudBlobStorageClients.Application.Tests.TestAwsS3
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestAwsS3CommandHandler : IRequestHandler<TestAwsS3Command>
    {
        private readonly IObjectStorage _objectStorage;
        private readonly ILogger<TestAwsS3CommandHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public TestAwsS3CommandHandler(IObjectStorage objectStorage, ILogger<TestAwsS3CommandHandler> logger)
        {
            _objectStorage = objectStorage;
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(TestAwsS3Command request, CancellationToken cancellationToken)
        {
            const string bucketName = "dan-test-bucket-4-24";

            Uri url;

            url = await _objectStorage.UploadStringAsync(bucketName, "test-file", "This is a test string", cancellationToken);
            _logger.LogInformation("UPLOAD: {Ur}", url);
            url = await _objectStorage.UploadStringAsync(url, "This is a test string 2", cancellationToken);
            _logger.LogInformation("UPLOAD: {Ur}", url);

            url = await _objectStorage.GetAsync(bucketName, "test-file", cancellationToken);
            _logger.LogInformation("GET: {Ur}", url);

            await foreach (var item in _objectStorage.ListAsync(bucketName, cancellationToken))
            {
                _logger.LogInformation("LIST: {Url}", item);
                var content = await _objectStorage.DownloadAsStringAsync(item, cancellationToken);
                _logger.LogInformation("CONTENT: {Content}", content);
            }

            await _objectStorage.DeleteAsync(bucketName, "test-file", cancellationToken);
        }
    }
}