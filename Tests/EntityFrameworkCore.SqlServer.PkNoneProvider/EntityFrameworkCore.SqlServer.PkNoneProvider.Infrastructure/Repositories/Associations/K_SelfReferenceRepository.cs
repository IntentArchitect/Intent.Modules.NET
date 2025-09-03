using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.Associations;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories.Associations;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class K_SelfReferenceRepository : RepositoryBase<K_SelfReference, K_SelfReference, ApplicationDbContext>, IK_SelfReferenceRepository
    {
        public K_SelfReferenceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<K_SelfReference?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<K_SelfReference?> FindByIdAsync(
            Guid id,
            Func<IQueryable<K_SelfReference>, IQueryable<K_SelfReference>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<K_SelfReference>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}