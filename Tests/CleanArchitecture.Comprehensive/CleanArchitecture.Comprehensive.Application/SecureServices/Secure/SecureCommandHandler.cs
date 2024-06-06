using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.SecureServices.Secure
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SecureCommandHandler : IRequestHandler<SecureCommand>
    {
        [IntentManaged(Mode.Merge)]
        public SecureCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(SecureCommand request, CancellationToken cancellationToken)
        {

        }
    }
}