using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArchDapr.TestApplication.Eventing.Messages;
using Subscribe.CleanArchDapr.TestApplication.Application.Common.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.Pubsub.EventHandler", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Application.IntegrationEventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomerCreatedHandler : IIntegrationEventHandler<CustomerCreatedEvent>
    {
        [IntentManaged(Mode.Merge)]
        public CustomerCreatedHandler()
        {
        }

        public async Task HandleAsync(CustomerCreatedEvent message, CancellationToken cancellationToken = default)
        {
        }
    }
}