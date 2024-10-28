using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.Headers.HeaderField
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class HeaderFieldCommandHandler : IRequestHandler<HeaderFieldCommand>
    {
        [IntentManaged(Mode.Merge)]
        public HeaderFieldCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(HeaderFieldCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (HeaderFieldCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}