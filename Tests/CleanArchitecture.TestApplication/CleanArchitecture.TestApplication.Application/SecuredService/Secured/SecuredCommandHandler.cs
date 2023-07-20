using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.SecuredService.Secured
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SecuredCommandHandler : IRequestHandler<SecuredCommand>
    {
        [IntentManaged(Mode.Merge)]
        public SecuredCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Unit> Handle(SecuredCommand request, CancellationToken cancellationToken)
        {
            // NOP
            return Unit.Value;
        }
    }
}