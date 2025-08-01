using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPH_FkAssociatedClassRepository : RepositoryBase<TPH_FkAssociatedClass, TPH_FkAssociatedClass, ApplicationDbContext>, ITPH_FkAssociatedClassRepository
    {
        public TPH_FkAssociatedClassRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPH_FkAssociatedClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<TPH_FkAssociatedClass?> FindByIdAsync(
            Guid id,
            Func<IQueryable<TPH_FkAssociatedClass>, IQueryable<TPH_FkAssociatedClass>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<TPH_FkAssociatedClass>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}