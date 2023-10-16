using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.ClassContainers.UpdateClassContainer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateClassContainerCommandHandler : IRequestHandler<UpdateClassContainerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateClassContainerCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(UpdateClassContainerCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}