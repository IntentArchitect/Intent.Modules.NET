using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureEventGrid.Application.Common.Eventing;
using AzureFunctions.AzureEventGrid.EventDomain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.AzureEventGrid.Application.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            _eventBus.Publish(new OrderCreatedEvent
            {
                RefNo = request.RefNo
            });
        }
    }
}