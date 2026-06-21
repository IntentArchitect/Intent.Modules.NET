using Intent.RoslynWeaver.Attributes;
using N_ServiceBus.AzureServiceBus.Application.Animals;
using N_ServiceBus.AzureServiceBus.Application.Common.Eventing;
using N_ServiceBus.AzureServiceBus.Application.Interfaces.Animals;
using N_ServiceBus.AzureServiceBus.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace N_ServiceBus.AzureServiceBus.Application.Implementation.Animals
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
        public async Task CreateAnimal(CreateAnimalDto dto, CancellationToken cancellationToken = default)
        {
            _messageBus.Send(new OrderAnimal
            {
                Name = dto.Name
            });
        }
    }
}