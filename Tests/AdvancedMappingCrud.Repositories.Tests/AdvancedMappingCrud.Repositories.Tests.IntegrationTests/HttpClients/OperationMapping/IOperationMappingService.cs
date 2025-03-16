using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.OperationMapping;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.OperationMapping
{
    public interface IOperationMappingService : IDisposable
    {
        Task CreateUserWithTaskItemAsync(CreateUserWithTaskItemCommand command, CancellationToken cancellationToken = default);
        Task CreateUserWithTaskItemContractAsync(CreateUserWithTaskItemContractCommand command, CancellationToken cancellationToken = default);
    }
}