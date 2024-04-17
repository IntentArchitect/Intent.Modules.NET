using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OpenApiImporterTest.Application.Stores.DeleteStoreOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteStoreOrderCommandHandler : IRequestHandler<DeleteStoreOrderCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteStoreOrderCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(DeleteStoreOrderCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}