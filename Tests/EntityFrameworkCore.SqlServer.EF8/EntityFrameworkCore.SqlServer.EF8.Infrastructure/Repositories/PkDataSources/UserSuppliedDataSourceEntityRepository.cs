using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.PkDataSources;
using EntityFrameworkCore.SqlServer.EF8.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF8.Domain.Repositories.PkDataSources;
using EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Repositories.PkDataSources
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UserSuppliedDataSourceEntityRepository : RepositoryBase<UserSuppliedDataSourceEntity, UserSuppliedDataSourceEntity, ApplicationDbContext>, IUserSuppliedDataSourceEntityRepository
    {
        public UserSuppliedDataSourceEntityRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<UserSuppliedDataSourceEntity?> FindByIdAsync(
            long id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<UserSuppliedDataSourceEntity?> FindByIdAsync(
            long id,
            Func<IQueryable<UserSuppliedDataSourceEntity>, IQueryable<UserSuppliedDataSourceEntity>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<UserSuppliedDataSourceEntity>> FindByIdsAsync(
            long[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}