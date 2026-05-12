using EventingSubscribers.Application.Common.Eventing;
using EventingSubscribers.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace EventingSubscribers.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ServiceOperationInvokedEventHandler : IIntegrationEventHandler<ServiceOperationInvokedEvent>
    {
        [IntentManaged(Mode.Merge)]
        public ServiceOperationInvokedEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(ServiceOperationInvokedEvent message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (ServiceOperationInvokedEventHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}