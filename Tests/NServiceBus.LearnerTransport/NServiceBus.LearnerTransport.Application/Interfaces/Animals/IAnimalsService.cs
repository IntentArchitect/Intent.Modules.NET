using Intent.RoslynWeaver.Attributes;
using NServiceBus.LearnerTransport.Application.Animals;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace NServiceBus.LearnerTransport.Application.Interfaces.Animals
{
    public interface IAnimalsService
    {
        Task CreateAnimal(CreateAnimalDto dto, CancellationToken cancellationToken = default);
    }
}