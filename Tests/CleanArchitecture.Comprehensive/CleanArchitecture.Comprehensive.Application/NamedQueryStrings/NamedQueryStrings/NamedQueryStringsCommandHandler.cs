using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.NamedQueryStrings.NamedQueryStrings
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class NamedQueryStringsCommandHandler : IRequestHandler<NamedQueryStringsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public NamedQueryStringsCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(NamedQueryStringsCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}