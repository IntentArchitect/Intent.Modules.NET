using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Indexes;
using EntityFrameworkCore.SqlServer.EF8.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF8.Domain.Repositories.Indexes;
using EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeviationIndexRepository : RepositoryBase<DeviationIndex, DeviationIndex, ApplicationDbContext>, IDeviationIndexRepository
    {
        public DeviationIndexRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<DeviationIndex?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<DeviationIndex?> FindByIdAsync(
            Guid id,
            Func<IQueryable<DeviationIndex>, IQueryable<DeviationIndex>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<DeviationIndex>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}