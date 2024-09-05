using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Put.CustomResponse203
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponse203Handler : IRequestHandler<CustomResponse203>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponse203Handler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(CustomResponse203 request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponse203Handler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}