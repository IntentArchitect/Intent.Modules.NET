using AdvancedMappingCrudMongo.Tests.IntegrationTests.Services;
using AdvancedMappingCrudMongo.Tests.IntegrationTests.Services.Basics;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests.HttpClients.Basics
{
    public interface IBasicsService : IDisposable
    {
        Task<string> CreateBasicAsync(CreateBasicCommand command, CancellationToken cancellationToken = default);
        Task DeleteBasicAsync(string id, CancellationToken cancellationToken = default);
        Task UpdateBasicAsync(string id, UpdateBasicCommand command, CancellationToken cancellationToken = default);
        Task<BasicDto> GetBasicByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<BasicDto>> GetBasicsAsync(int pageNo, int pageSize, string orderBy, CancellationToken cancellationToken = default);
    }
}