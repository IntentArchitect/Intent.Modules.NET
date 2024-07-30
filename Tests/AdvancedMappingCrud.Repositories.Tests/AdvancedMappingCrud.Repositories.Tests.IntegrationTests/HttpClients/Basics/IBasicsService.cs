using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Basics;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.Basics
{
    public interface IBasicsService : IDisposable
    {
        Task<Guid> CreateBasicAsync(CreateBasicCommand command, CancellationToken cancellationToken = default);
        Task DeleteBasicAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateBasicAsync(Guid id, UpdateBasicCommand command, CancellationToken cancellationToken = default);
        Task<BasicDto> GetBasicByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<BasicDto>> GetBasicsNullableAsync(int pageNo, int pageSize, string? orderBy, CancellationToken cancellationToken = default);
        Task<PagedResult<BasicDto>> GetBasicsAsync(int pageNo, int pageSize, string orderBy, CancellationToken cancellationToken = default);
    }
}