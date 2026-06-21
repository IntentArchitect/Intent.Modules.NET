using Intent.RoslynWeaver.Attributes;
using N_ServiceBus.LearnerTransport.Application.Animals;
using N_ServiceBus.LearnerTransport.Application.Common.Eventing;
using N_ServiceBus.LearnerTransport.Application.Interfaces.Animals;
using N_ServiceBus.LearnerTransport.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace N_ServiceBus.LearnerTransport.Application.Implementation.Animals
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
                Name = dto.Name,
                Type = dto.Type
            });
        }
    }
}