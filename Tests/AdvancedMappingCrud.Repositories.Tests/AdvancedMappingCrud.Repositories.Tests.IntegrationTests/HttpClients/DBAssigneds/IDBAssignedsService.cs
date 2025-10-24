using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.DBAssigneds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.DBAssigneds
{
    public interface IDBAssignedsService : IDisposable
    {
        Task<Guid> CreateDBAssignedAsync(CreateDBAssignedCommand command, CancellationToken cancellationToken = default);
        Task DeleteDBAssignedAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateDBAssignedAsync(Guid id, UpdateDBAssignedCommand command, CancellationToken cancellationToken = default);
        Task<DBAssignedDto> GetDBAssignedByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<DBAssignedDto>> GetDBAssignedsAsync(CancellationToken cancellationToken = default);
    }
}