using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.TPC.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPC_FkBaseClassAssociatedRepository : RepositoryBase<TPC_FkBaseClassAssociated, TPC_FkBaseClassAssociated, ApplicationDbContext>, ITPC_FkBaseClassAssociatedRepository
    {
        public TPC_FkBaseClassAssociatedRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPC_FkBaseClassAssociated?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<TPC_FkBaseClassAssociated?> FindByIdAsync(
            Guid id,
            Func<IQueryable<TPC_FkBaseClassAssociated>, IQueryable<TPC_FkBaseClassAssociated>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<TPC_FkBaseClassAssociated>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}