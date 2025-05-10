using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.PTH.Application.IntegrationServices;
using ProxyServiceTests.Proxy.PTH.Application.IntegrationServices.Contracts.OriginalServices.Services.File;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.File.FileUpload
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FileUploadCommandHandler : IRequestHandler<FileUploadCommand>
    {
        private readonly IFileService _fileService;

        [IntentManaged(Mode.Merge)]
        public FileUploadCommandHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(FileUploadCommand request, CancellationToken cancellationToken)
        {
            await _fileService.FileUploadAsync(
                request.ContentType,
                request.ContentLength,
                new IntegrationServices.Contracts.OriginalServices.Services.File.FileUploadCommand
                {
                    Content = request.Content,
                    Filename = request.Filename,
                    ContentType = request.ContentType,
                    ContentLength = request.ContentLength
                },
                cancellationToken);
        }
    }
}