using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Regions.CreateRegion
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateRegionCommandHandler : IRequestHandler<CreateRegionCommand, string>
    {
        [IntentManaged(Mode.Merge)]
        public CreateRegionCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<string> Handle(CreateRegionCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}