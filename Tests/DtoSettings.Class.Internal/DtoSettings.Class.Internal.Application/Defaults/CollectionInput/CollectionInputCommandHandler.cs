using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace DtoSettings.Class.Internal.Application.Defaults.CollectionInput
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CollectionInputCommandHandler : IRequestHandler<CollectionInputCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CollectionInputCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(CollectionInputCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CollectionInputCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}