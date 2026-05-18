using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IContainerRepository : IEFRepository<Container, Container>
    {
        [IntentManaged(Mode.Fully)]
        Task<Container?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Container?> FindByIdAsync(Guid id, Func<IQueryable<Container>, IQueryable<Container>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Container>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}