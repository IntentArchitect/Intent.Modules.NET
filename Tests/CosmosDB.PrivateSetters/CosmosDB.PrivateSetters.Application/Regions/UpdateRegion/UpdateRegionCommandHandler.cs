using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Regions.UpdateRegion
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateRegionCommandHandler : IRequestHandler<UpdateRegionCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateRegionCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(UpdateRegionCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (UpdateRegionCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}