using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace HashiCorpVault.Application.VaultTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class VaultTestCommandHandler : IRequestHandler<VaultTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public VaultTestCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(VaultTestCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (VaultTestCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}