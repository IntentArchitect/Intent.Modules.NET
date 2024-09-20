using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArchDapr.TestApplication.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Pubsub.EventHandlerImplementation", Version = "2.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Application.IntegrationEventHandlers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomerCreatedEventHandler : IRequestHandler<CustomerCreatedEvent>
    {
        [IntentManaged(Mode.Ignore)]
        public CustomerCreatedEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(CustomerCreatedEvent @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}