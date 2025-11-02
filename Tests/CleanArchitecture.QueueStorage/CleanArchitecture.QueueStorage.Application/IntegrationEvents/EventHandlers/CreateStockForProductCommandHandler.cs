using CleanArchitecture.QueueStorage.Application.Common.Eventing;
using CleanArchitecture.QueueStorage.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureQueueStorage.IntegrationEventHandler", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateStockForProductCommandHandler : IIntegrationEventHandler<CreateStockForProductCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateStockForProductCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(CreateStockForProductCommand message, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("CreateStockForProductCommand processed");
        }
    }
}