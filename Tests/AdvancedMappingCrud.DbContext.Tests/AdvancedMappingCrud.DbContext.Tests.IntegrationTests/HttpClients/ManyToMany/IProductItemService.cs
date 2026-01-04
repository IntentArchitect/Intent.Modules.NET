using AdvancedMappingCrud.DbContext.Tests.IntegrationTests.Services.ManyToMany;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests.HttpClients.ManyToMany
{
    public interface IProductItemService : IDisposable
    {
        Task CreateProductItemAsync(CreateProductItemDto dto, CancellationToken cancellationToken = default);
        Task UpdateProductItemAsync(UpdateProductItemDto dto, Guid id, CancellationToken cancellationToken = default);
    }
}