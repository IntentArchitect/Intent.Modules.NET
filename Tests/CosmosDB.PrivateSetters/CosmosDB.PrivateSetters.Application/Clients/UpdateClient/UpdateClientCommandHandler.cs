using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Clients.UpdateClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateClientCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}