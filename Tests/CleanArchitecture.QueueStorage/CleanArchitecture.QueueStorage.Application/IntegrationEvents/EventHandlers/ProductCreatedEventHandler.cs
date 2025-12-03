using AzureFunction.QueueStorage.Eventing.Messages;
using CleanArchitecture.QueueStorage.Application.Common.Eventing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ProductCreatedEventHandler : IIntegrationEventHandler<ProductCreatedEvent>
    {
        [IntentManaged(Mode.Merge)]
        public ProductCreatedEventHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(ProductCreatedEvent message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("ProductCreatedEvent processed");
        }
    }
}