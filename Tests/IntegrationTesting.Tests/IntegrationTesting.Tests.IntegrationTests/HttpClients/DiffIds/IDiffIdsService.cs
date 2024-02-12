using IntegrationTesting.Tests.IntegrationTests.Services.DiffIds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.IntegrationTests.HttpClients.DiffIds
{
    public interface IDiffIdsService : IDisposable
    {
        Task<Guid> CreateDiffIdAsync(CreateDiffIdCommand command, CancellationToken cancellationToken = default);
        Task DeleteDiffIdAsync(Guid myId, CancellationToken cancellationToken = default);
        Task UpdateDiffIdAsync(Guid myId, UpdateDiffIdCommand command, CancellationToken cancellationToken = default);
        Task<DiffIdDto> GetDiffIdByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<DiffIdDto>> GetDiffIdsAsync(CancellationToken cancellationToken = default);
    }
}