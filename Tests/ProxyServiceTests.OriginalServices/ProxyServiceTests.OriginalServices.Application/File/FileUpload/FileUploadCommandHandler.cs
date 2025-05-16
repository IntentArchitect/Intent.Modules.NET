using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ProxyServiceTests.OriginalServices.Application.File.FileUpload
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FileUploadCommandHandler : IRequestHandler<FileUploadCommand>
    {
        [IntentManaged(Mode.Merge)]
        public FileUploadCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(FileUploadCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (FileUploadCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}