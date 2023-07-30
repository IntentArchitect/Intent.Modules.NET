using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneDests.CreateManyToOneDest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateManyToOneDestCommandHandler : IRequestHandler<CreateManyToOneDestCommand, Guid>
    {
        [IntentManaged(Mode.Merge)]
        public CreateManyToOneDestCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(CreateManyToOneDestCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}