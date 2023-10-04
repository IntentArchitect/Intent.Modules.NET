using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.CosmosDB.CosmosChange
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CosmosChangeCommandHandler : IRequestHandler<CosmosChangeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CosmosChangeCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(CosmosChangeCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}