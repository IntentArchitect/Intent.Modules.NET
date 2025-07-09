using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.NamedQueryStrings.NamedQueryStrings
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class NamedQueryStringsCommandHandler : IRequestHandler<NamedQueryStringsCommand>
    {
        [IntentManaged(Mode.Merge)]
        public NamedQueryStringsCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(NamedQueryStringsCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (NamedQueryStringsCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}