using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Validation.InboundValidation
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class InboundValidationCommandHandler : IRequestHandler<InboundValidationCommand>
    {
        [IntentManaged(Mode.Merge)]
        public InboundValidationCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(InboundValidationCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}