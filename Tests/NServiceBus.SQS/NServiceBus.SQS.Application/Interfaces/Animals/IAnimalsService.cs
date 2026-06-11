using Intent.RoslynWeaver.Attributes;
using NServiceBus.SQS.Application.Animals;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace NServiceBus.SQS.Application.Interfaces.Animals
{
    public interface IAnimalsService
    {
        Task CreateAnimal(CreateAnimalDto dto, CancellationToken cancellationToken = default);
    }
}