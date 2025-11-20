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
    public class ParentNonStdIdRepository : RepositoryBase<ParentNonStdId, ParentNonStdId, ApplicationDbContext>, IParentNonStdIdRepository
    {
        public ParentNonStdIdRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ParentNonStdId?> FindByIdAsync(Guid myId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.MyId == myId, cancellationToken);
        }

        public async Task<ParentNonStdId?> FindByIdAsync(
            Guid myId,
            Func<IQueryable<ParentNonStdId>, IQueryable<ParentNonStdId>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.MyId == myId, queryOptions, cancellationToken);
        }

        public async Task<List<ParentNonStdId>> FindByIdsAsync(Guid[] myIds, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = myIds.ToList();
            return await FindAllAsync(x => idList.Contains(x.MyId), cancellationToken);
        }
    }
}