using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF10.Domain.Entities.PkDataSources;
using EntityFrameworkCore.SqlServer.EF10.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF10.Domain.Repositories.PkDataSources;
using EntityFrameworkCore.SqlServer.EF10.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Infrastructure.Repositories.PkDataSources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CompKeyDefaultDataSourceEntityRepository : RepositoryBase<CompKeyDefaultDataSourceEntity, CompKeyDefaultDataSourceEntity, ApplicationDbContext>, ICompKeyDefaultDataSourceEntityRepository
    {
        public CompKeyDefaultDataSourceEntityRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<CompKeyDefaultDataSourceEntity?> FindByIdAsync(
            (long Id1, long Id2) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id1 == id.Id1 && x.Id2 == id.Id2, cancellationToken);
        }

        public async Task<CompKeyDefaultDataSourceEntity?> FindByIdAsync(
            (long Id1, long Id2) id,
            Func<IQueryable<CompKeyDefaultDataSourceEntity>, IQueryable<CompKeyDefaultDataSourceEntity>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id1 == id.Id1 && x.Id2 == id.Id2, queryOptions, cancellationToken);
        }
    }
}