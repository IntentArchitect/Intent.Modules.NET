using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.TPT.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories.TPT.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Repositories.TPT.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPT_AbstractBaseClassRepository : RepositoryBase<TPT_AbstractBaseClass, TPT_AbstractBaseClass, ApplicationDbContext>, ITPT_AbstractBaseClassRepository
    {
        public TPT_AbstractBaseClassRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPT_AbstractBaseClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<TPT_AbstractBaseClass?> FindByIdAsync(
            Guid id,
            Func<IQueryable<TPT_AbstractBaseClass>, IQueryable<TPT_AbstractBaseClass>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<TPT_AbstractBaseClass>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}