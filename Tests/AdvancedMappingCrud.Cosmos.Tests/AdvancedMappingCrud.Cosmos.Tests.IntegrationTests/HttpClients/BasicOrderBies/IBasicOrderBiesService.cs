using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services;
using AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.BasicOrderBies;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients.BasicOrderBies
{
    public interface IBasicOrderBiesService : IDisposable
    {
        Task<string> CreateBasicOrderByAsync(CreateBasicOrderByCommand command, CancellationToken cancellationToken = default);
        Task DeleteBasicOrderByAsync(string id, CancellationToken cancellationToken = default);
        Task UpdateBasicOrderByAsync(string id, UpdateBasicOrderByCommand command, CancellationToken cancellationToken = default);
        Task<BasicOrderByDto> GetBasicOrderByByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<BasicOrderByDto>> GetBasicOrderByAsync(int pageNo, int pageSize, string orderBy, CancellationToken cancellationToken = default);
    }
}