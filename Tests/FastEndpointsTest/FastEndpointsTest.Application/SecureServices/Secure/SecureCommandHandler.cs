using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.SecureServices.Secure
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SecureCommandHandler : IRequestHandler<SecureCommand>
    {
        [IntentManaged(Mode.Merge)]
        public SecureCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(SecureCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (SecureCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}