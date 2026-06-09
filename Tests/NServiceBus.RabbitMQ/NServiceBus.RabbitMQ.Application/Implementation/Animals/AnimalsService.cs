using Intent.RoslynWeaver.Attributes;
using NServiceBus.RabbitMQ.Application.Common.Eventing;
using NServiceBus.RabbitMQ.Application.Interfaces.Animals;
using NServiceBus.RabbitMQ.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace NServiceBus.RabbitMQ.Application.Implementation.Animals
{
    [IntentManaged(Mode.Merge)]
    public class AnimalsService : IAnimalsService
    {
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public AnimalsService(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CreateAnimal(string dto, CancellationToken cancellationToken = default)
        {
            _messageBus.Send(new OrderAnimal
            {
            });
        }
    }
}