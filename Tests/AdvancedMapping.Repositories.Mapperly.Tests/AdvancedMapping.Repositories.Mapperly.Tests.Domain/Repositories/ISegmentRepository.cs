using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ISegmentRepository : IEFRepository<Segment, Segment>
    {
        [IntentManaged(Mode.Fully)]
        Task<Segment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Segment?> FindByIdAsync(Guid id, Func<IQueryable<Segment>, IQueryable<Segment>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Segment>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}