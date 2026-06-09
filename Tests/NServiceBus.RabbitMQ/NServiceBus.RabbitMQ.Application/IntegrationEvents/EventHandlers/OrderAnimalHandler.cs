using Intent.RoslynWeaver.Attributes;
using NServiceBus.RabbitMQ.Application.Common.Eventing;
using NServiceBus.RabbitMQ.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace NServiceBus.RabbitMQ.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class OrderAnimalHandler : IIntegrationEventHandler<OrderAnimal>, IIntegrationEventHandler<MakeSoundCommand>, IIntegrationEventHandler<TalkToPersonCommand>, IIntegrationEventHandler<CreatePersonIdentity>
    {
        [IntentManaged(Mode.Merge)]
        public OrderAnimalHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(OrderAnimal message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (OrderAnimalHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(MakeSoundCommand message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (OrderAnimalHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(TalkToPersonCommand message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (OrderAnimalHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task HandleAsync(CreatePersonIdentity message, CancellationToken cancellationToken = default)
        {
            // TODO: Implement HandleAsync (OrderAnimalHandler) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}