using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Patch.CustomResponse205
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse205Handler : IRequestHandler<CustomResponse205>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse205Handler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(CustomResponse205 request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse205Handler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}