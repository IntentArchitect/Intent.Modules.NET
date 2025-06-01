using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.PkDataSources;
using EntityFrameworkCore.Postgres.Domain.Repositories;
using EntityFrameworkCore.Postgres.Domain.Repositories.PkDataSources;
using EntityFrameworkCore.Postgres.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Infrastructure.Repositories.PkDataSources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CompKeyUserSuppliedDataSourceEntityRepository : RepositoryBase<CompKeyUserSuppliedDataSourceEntity, CompKeyUserSuppliedDataSourceEntity, ApplicationDbContext>, ICompKeyUserSuppliedDataSourceEntityRepository
    {
        public CompKeyUserSuppliedDataSourceEntityRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<CompKeyUserSuppliedDataSourceEntity?> FindByIdAsync(
            (long Id1, long Id2) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id1 == id.Id1 && x.Id2 == id.Id2, cancellationToken);
        }

        public async Task<CompKeyUserSuppliedDataSourceEntity?> FindByIdAsync(
            (long Id1, long Id2) id,
            Func<IQueryable<CompKeyUserSuppliedDataSourceEntity>, IQueryable<CompKeyUserSuppliedDataSourceEntity>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id1 == id.Id1 && x.Id2 == id.Id2, queryOptions, cancellationToken);
        }
    }
}