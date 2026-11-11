using EntityFrameworkCore.SqlServer.EF11.Domain.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IP_SourceNameDiffRepository : IEFRepository<P_SourceNameDiff, P_SourceNameDiff>
    {
        [IntentManaged(Mode.Fully)]
        Task<P_SourceNameDiff?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<P_SourceNameDiff?> FindByIdAsync(Guid id, Func<IQueryable<P_SourceNameDiff>, IQueryable<P_SourceNameDiff>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<P_SourceNameDiff>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}