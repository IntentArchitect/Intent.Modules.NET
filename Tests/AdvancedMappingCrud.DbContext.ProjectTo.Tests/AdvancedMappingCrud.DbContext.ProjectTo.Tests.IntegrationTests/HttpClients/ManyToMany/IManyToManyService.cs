using AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.Tests.Services.ManyToMany;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.IntegrationTests.HttpClients.ManyToMany
{
    public interface IManyToManyService : IDisposable
    {
        Task CreateProductItemAsync(CreateProductItemCommand command, CancellationToken cancellationToken = default);
        Task UpdateProductItemAsync(Guid id, UpdateProductItemCommand command, CancellationToken cancellationToken = default);
    }
}