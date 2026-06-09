using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace NServiceBus.AzureServiceBus.Application.Interfaces.Animals
{
    public interface IAnimalsService
    {
        Task CreateAnimal(string dto, CancellationToken cancellationToken = default);
    }
}