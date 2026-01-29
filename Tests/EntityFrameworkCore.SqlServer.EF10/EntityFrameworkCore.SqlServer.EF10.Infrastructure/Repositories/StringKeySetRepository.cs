using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF10.Domain.Entities;
using EntityFrameworkCore.SqlServer.EF10.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF10.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class StringKeySetRepository : RepositoryBase<StringKeySet, StringKeySet, ApplicationDbContext>, IStringKeySetRepository
    {
        public StringKeySetRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<StringKeySet?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<StringKeySet?> FindByIdAsync(
            string id,
            Func<IQueryable<StringKeySet>, IQueryable<StringKeySet>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<StringKeySet>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}