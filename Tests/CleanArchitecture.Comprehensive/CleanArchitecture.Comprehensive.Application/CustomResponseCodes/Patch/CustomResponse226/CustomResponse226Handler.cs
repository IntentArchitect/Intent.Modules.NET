using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Patch.CustomResponse226
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse226Handler : IRequestHandler<CustomResponse226>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse226Handler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(CustomResponse226 request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse226Handler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}