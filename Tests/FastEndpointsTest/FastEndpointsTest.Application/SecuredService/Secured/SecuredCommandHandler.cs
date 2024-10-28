using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.SecuredService.Secured
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SecuredCommandHandler : IRequestHandler<SecuredCommand>
    {
        [IntentManaged(Mode.Merge)]
        public SecuredCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(SecuredCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (SecuredCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}