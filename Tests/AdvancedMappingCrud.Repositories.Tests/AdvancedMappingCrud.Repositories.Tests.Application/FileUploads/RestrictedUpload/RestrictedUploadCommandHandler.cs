using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.RestrictedUpload
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RestrictedUploadCommandHandler : IRequestHandler<RestrictedUploadCommand>
    {
        [IntentManaged(Mode.Merge)]
        public RestrictedUploadCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(RestrictedUploadCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}