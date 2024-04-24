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
        private readonly IAwsS3BlobStorage _aws;
        private readonly ILogger<TestAwsS3CommandHandler> _logger;

        [IntentManaged(Mode.Merge)]
        public TestAwsS3CommandHandler(IAwsS3BlobStorage aws, ILogger<TestAwsS3CommandHandler> logger)
        {
            _aws = aws;
            _logger = logger;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(TestAwsS3Command request, CancellationToken cancellationToken)
        {
            const string bucketName = "dan-test-bucket-4-24";

            var url = await _aws.UploadStringAsync(bucketName, "test-file", "This is a test string", cancellationToken);
            _logger.LogInformation("UPLOAD: {Ur}", url);

            var url2 = await _aws.GetAsync(bucketName, "test-file", cancellationToken);
            _logger.LogInformation("GET: {Ur}", url2);

            await foreach (var item in _aws.ListAsync(bucketName, cancellationToken))
            {
                _logger.LogInformation("LIST: {Url}", item);
                var content = await _aws.DownloadAsStringAsync(item, cancellationToken);
                _logger.LogInformation("CONTENT: {Content}", content);
            }

            await _aws.DeleteAsync(bucketName, "test-file", cancellationToken);
        }
    }
}