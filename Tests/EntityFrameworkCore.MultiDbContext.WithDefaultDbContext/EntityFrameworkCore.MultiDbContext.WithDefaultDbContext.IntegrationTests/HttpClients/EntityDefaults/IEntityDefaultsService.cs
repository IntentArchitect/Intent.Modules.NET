using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityDefaults;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityDefaults
{
    public interface IEntityDefaultsService : IDisposable
    {
        Task<Guid> CreateEntityDefaultAsync(CreateEntityDefaultCommand command, CancellationToken cancellationToken = default);
        Task DeleteEntityDefaultAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateEntityDefaultAsync(Guid id, UpdateEntityDefaultCommand command, CancellationToken cancellationToken = default);
        Task<EntityDefaultDto> GetEntityDefaultByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<EntityDefaultDto>> GetEntityDefaultsAsync(CancellationToken cancellationToken = default);
    }
}