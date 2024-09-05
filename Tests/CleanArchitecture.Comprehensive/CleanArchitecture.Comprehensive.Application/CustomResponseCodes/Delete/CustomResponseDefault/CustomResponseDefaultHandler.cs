using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.CustomResponseCodes.Delete.CustomResponseDefault
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomResponseDefaultHandler : IRequestHandler<CustomResponseDefault>
    {
        [IntentManaged(Mode.Merge)]
        public CustomResponseDefaultHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(CustomResponseDefault request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CustomResponseDefaultHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}