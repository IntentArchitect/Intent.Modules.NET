using EntityFrameworkCore.SqlServer.EF11.Domain.Indexes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ISortDirectionIndexRepository : IEFRepository<SortDirectionIndex, SortDirectionIndex>
    {
        [IntentManaged(Mode.Fully)]
        Task<SortDirectionIndex?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<SortDirectionIndex?> FindByIdAsync(Guid id, Func<IQueryable<SortDirectionIndex>, IQueryable<SortDirectionIndex>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<SortDirectionIndex>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}