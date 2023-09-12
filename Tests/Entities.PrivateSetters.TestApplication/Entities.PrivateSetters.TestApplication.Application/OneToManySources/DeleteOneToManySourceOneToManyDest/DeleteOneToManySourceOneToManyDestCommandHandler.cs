using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.DeleteOneToManySourceOneToManyDest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteOneToManySourceOneToManyDestCommandHandler : IRequestHandler<DeleteOneToManySourceOneToManyDestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteOneToManySourceOneToManyDestCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(DeleteOneToManySourceOneToManyDestCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}