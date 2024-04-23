using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.ExplicitKeys;
using EntityFrameworkCore.SqlServer.EF8.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF8.Domain.Repositories.ExplicitKeys;
using EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Repositories.ExplicitKeys
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

        public async Task<List<PK_PrimaryKeyInt>> FindByIdsAsync(
            int[] primaryKeyIds,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => primaryKeyIds.Contains(x.PrimaryKeyId), cancellationToken);
        }
    }
}