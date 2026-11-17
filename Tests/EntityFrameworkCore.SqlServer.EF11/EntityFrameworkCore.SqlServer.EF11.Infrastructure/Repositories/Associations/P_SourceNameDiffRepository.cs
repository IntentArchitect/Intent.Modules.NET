using EntityFrameworkCore.SqlServer.EF11.Domain.Associations;
using EntityFrameworkCore.SqlServer.EF11.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.Associations;
using EntityFrameworkCore.SqlServer.EF11.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Infrastructure.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class P_SourceNameDiffRepository : RepositoryBase<P_SourceNameDiff, P_SourceNameDiff, ApplicationDbContext>, IP_SourceNameDiffRepository
    {
        public P_SourceNameDiffRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<P_SourceNameDiff?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<P_SourceNameDiff?> FindByIdAsync(
            Guid id,
            Func<IQueryable<P_SourceNameDiff>, IQueryable<P_SourceNameDiff>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<P_SourceNameDiff>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}