using EntityFrameworkCore.SqlServer.EF11.Domain;
using EntityFrameworkCore.SqlServer.EF11.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF11.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DecimalKeyDefaultRepository : RepositoryBase<DecimalKeyDefault, DecimalKeyDefault, ApplicationDbContext>, IDecimalKeyDefaultRepository
    {
        public DecimalKeyDefaultRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<DecimalKeyDefault?> FindByIdAsync(decimal id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<DecimalKeyDefault?> FindByIdAsync(
            decimal id,
            Func<IQueryable<DecimalKeyDefault>, IQueryable<DecimalKeyDefault>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<DecimalKeyDefault>> FindByIdsAsync(
            decimal[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}