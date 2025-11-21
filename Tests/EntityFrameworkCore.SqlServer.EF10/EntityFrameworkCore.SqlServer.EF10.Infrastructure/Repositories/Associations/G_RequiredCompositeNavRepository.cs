using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF10.Domain.Entities.Associations;
using EntityFrameworkCore.SqlServer.EF10.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF10.Domain.Repositories.Associations;
using EntityFrameworkCore.SqlServer.EF10.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Infrastructure.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class G_RequiredCompositeNavRepository : RepositoryBase<G_RequiredCompositeNav, G_RequiredCompositeNav, ApplicationDbContext>, IG_RequiredCompositeNavRepository
    {
        public G_RequiredCompositeNavRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<G_RequiredCompositeNav?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<G_RequiredCompositeNav?> FindByIdAsync(
            Guid id,
            Func<IQueryable<G_RequiredCompositeNav>, IQueryable<G_RequiredCompositeNav>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<G_RequiredCompositeNav>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}