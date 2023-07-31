using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.Validation.Validated
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ValidatedCommandHandler : IRequestHandler<ValidatedCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ValidatedCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(ValidatedCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}