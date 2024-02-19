using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.UploadFile
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UploadFileHandler : IRequestHandler<UploadFile, Guid>
    {
        [IntentManaged(Mode.Merge)]
        public UploadFileHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(UploadFile request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}