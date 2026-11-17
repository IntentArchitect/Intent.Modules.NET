using EntityFrameworkCore.SqlServer.EF11.Domain.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IQ_DestNameDiffRepository : IEFRepository<Q_DestNameDiff, Q_DestNameDiff>
    {
        [IntentManaged(Mode.Fully)]
        Task<Q_DestNameDiff?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Q_DestNameDiff?> FindByIdAsync(Guid id, Func<IQueryable<Q_DestNameDiff>, IQueryable<Q_DestNameDiff>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Q_DestNameDiff>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}