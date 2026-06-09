using Intent.RoslynWeaver.Attributes;
using NServiceBus.AzureServiceBus.Application.Common.Eventing;
using NServiceBus.AzureServiceBus.Application.Interfaces.Animals;
using NServiceBus.AzureServiceBus.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace NServiceBus.AzureServiceBus.Application.Implementation.Animals
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