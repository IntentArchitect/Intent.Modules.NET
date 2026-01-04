using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.ManyToMany;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.ManyToMany
{
    public interface IManyToManyService : IDisposable
    {
        Task CreateProductItemAsync(CreateProductItemCommand command, CancellationToken cancellationToken = default);
        Task UpdateProductItemAsync(Guid id, UpdateProductItemCommand command, CancellationToken cancellationToken = default);
    }
}