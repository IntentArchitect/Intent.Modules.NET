using Intent.RoslynWeaver.Attributes;
using NServiceBus.LearnerTransport.Application.Animals;
using NServiceBus.LearnerTransport.Application.Common.Eventing;
using NServiceBus.LearnerTransport.Application.Interfaces.Animals;
using NServiceBus.LearnerTransport.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace NServiceBus.LearnerTransport.Application.Implementation.Animals
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
            });
        }
    }
}