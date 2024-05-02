using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.Services.EntityAlternates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.IntegrationTests.HttpClients.EntityAlternates
{
    public interface IEntityAlternatesService : IDisposable
    {
        Task<Guid> CreateEntityAlternateAsync(CreateEntityAlternateCommand command, CancellationToken cancellationToken = default);
        Task DeleteEntityAlternateAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateEntityAlternateAsync(Guid id, UpdateEntityAlternateCommand command, CancellationToken cancellationToken = default);
        Task<EntityAlternateDto> GetEntityAlternateByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<EntityAlternateDto>> GetEntityAlternatesAsync(CancellationToken cancellationToken = default);
    }
}