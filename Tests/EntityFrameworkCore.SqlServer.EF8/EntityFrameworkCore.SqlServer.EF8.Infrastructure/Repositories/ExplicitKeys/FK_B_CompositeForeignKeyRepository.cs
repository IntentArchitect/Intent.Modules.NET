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
    public class FK_B_CompositeForeignKeyRepository : RepositoryBase<FK_B_CompositeForeignKey, FK_B_CompositeForeignKey, ApplicationDbContext>, IFK_B_CompositeForeignKeyRepository
    {
        public FK_B_CompositeForeignKeyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<FK_B_CompositeForeignKey?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<FK_B_CompositeForeignKey>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}