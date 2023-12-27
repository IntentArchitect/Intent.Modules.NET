using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Eventing;
using AdvancedMappingCrud.Repositories.Tests.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.IntegrationEvents.EventHandlers.Customers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class QuoteCreatedIntegrationEventHandler : IIntegrationEventHandler<QuoteCreatedIntegrationEvent>
    {
        [IntentManaged(Mode.Merge)]
        public QuoteCreatedIntegrationEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(QuoteCreatedIntegrationEvent message, CancellationToken cancellationToken = default)
        {
        }
    }
}