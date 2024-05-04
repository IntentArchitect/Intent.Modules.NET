using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityAppDefaults;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityAppDefaults
{
    public interface IEntityAppDefaultsService : IDisposable
    {
        Task<Guid> CreateEntityAppDefaultAsync(CreateEntityAppDefaultCommand command, CancellationToken cancellationToken = default);
        Task DeleteEntityAppDefaultAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateEntityAppDefaultAsync(Guid id, UpdateEntityAppDefaultCommand command, CancellationToken cancellationToken = default);
        Task<EntityAppDefaultDto> GetEntityAppDefaultByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<EntityAppDefaultDto>> GetEntityAppDefaultsAsync(CancellationToken cancellationToken = default);
    }
}