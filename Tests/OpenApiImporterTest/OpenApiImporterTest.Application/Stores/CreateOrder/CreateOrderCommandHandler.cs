using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OpenApiImporterTest.Application.Stores.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
    {
        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CreateOrderCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}