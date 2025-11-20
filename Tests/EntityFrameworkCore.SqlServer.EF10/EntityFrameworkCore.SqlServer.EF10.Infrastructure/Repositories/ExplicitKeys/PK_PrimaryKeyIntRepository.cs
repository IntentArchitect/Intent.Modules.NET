using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF10.Domain.Entities.ExplicitKeys;
using EntityFrameworkCore.SqlServer.EF10.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF10.Domain.Repositories.ExplicitKeys;
using EntityFrameworkCore.SqlServer.EF10.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Infrastructure.Repositories.ExplicitKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PK_PrimaryKeyIntRepository : RepositoryBase<PK_PrimaryKeyInt, PK_PrimaryKeyInt, ApplicationDbContext>, IPK_PrimaryKeyIntRepository
    {
        public PK_PrimaryKeyIntRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PK_PrimaryKeyInt?> FindByIdAsync(int primaryKeyId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.PrimaryKeyId == primaryKeyId, cancellationToken);
        }

        public async Task<PK_PrimaryKeyInt?> FindByIdAsync(
            int primaryKeyId,
            Func<IQueryable<PK_PrimaryKeyInt>, IQueryable<PK_PrimaryKeyInt>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.PrimaryKeyId == primaryKeyId, queryOptions, cancellationToken);
        }

        public async Task<List<PK_PrimaryKeyInt>> FindByIdsAsync(
            int[] primaryKeyIds,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = primaryKeyIds.ToList();
            return await FindAllAsync(x => idList.Contains(x.PrimaryKeyId), cancellationToken);
        }
    }
}