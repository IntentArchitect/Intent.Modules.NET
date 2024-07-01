using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OpenApiImporterTest.Application.Stores.DeleteOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteOrderCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (DeleteOrderCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}