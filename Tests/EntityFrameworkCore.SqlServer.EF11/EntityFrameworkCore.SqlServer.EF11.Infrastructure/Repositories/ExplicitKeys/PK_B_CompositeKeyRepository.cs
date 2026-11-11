using EntityFrameworkCore.SqlServer.EF11.Domain.ExplicitKeys;
using EntityFrameworkCore.SqlServer.EF11.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF11.Domain.Repositories.ExplicitKeys;
using EntityFrameworkCore.SqlServer.EF11.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Infrastructure.Repositories.ExplicitKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PK_B_CompositeKeyRepository : RepositoryBase<PK_B_CompositeKey, PK_B_CompositeKey, ApplicationDbContext>, IPK_B_CompositeKeyRepository
    {
        public PK_B_CompositeKeyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PK_B_CompositeKey?> FindByIdAsync(
            (Guid CompositeKeyA, Guid CompositeKeyB) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.CompositeKeyA == id.CompositeKeyA && x.CompositeKeyB == id.CompositeKeyB, cancellationToken);
        }

        public async Task<PK_B_CompositeKey?> FindByIdAsync(
            (Guid CompositeKeyA, Guid CompositeKeyB) id,
            Func<IQueryable<PK_B_CompositeKey>, IQueryable<PK_B_CompositeKey>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.CompositeKeyA == id.CompositeKeyA && x.CompositeKeyB == id.CompositeKeyB, queryOptions, cancellationToken);
        }
    }
}