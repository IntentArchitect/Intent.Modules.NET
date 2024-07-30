using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.PagingTS;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients
{
    public interface IPagingTSService : IDisposable
    {
        Task<Guid> CreatePagingTSAsync(PagingTSCreateDto dto, CancellationToken cancellationToken = default);
        Task<PagingTSDto> FindPagingTSByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<PagingTSDto>> FindPagingTSAsync(int pageNo, int pageSize, string? orderBy, CancellationToken cancellationToken = default);
        Task UpdatePagingTSAsync(Guid id, PagingTSUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeletePagingTSAsync(Guid id, CancellationToken cancellationToken = default);
    }
}