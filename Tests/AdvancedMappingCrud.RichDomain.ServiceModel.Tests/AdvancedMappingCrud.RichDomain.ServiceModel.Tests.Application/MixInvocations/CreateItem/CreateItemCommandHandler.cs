using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.MixInvocations.CreateItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Guid>
    {
        [IntentManaged(Mode.Merge)]
        public CreateItemCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<Guid> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CreateItemCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}