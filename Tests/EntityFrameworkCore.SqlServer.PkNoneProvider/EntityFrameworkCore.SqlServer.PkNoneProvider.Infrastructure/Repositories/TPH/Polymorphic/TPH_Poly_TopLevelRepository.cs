using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.TPH.Polymorphic;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories.TPH.Polymorphic;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Repositories.TPH.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPH_Poly_TopLevelRepository : RepositoryBase<TPH_Poly_TopLevel, TPH_Poly_TopLevel, ApplicationDbContext>, ITPH_Poly_TopLevelRepository
    {
        public TPH_Poly_TopLevelRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPH_Poly_TopLevel?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<TPH_Poly_TopLevel?> FindByIdAsync(
            Guid id,
            Func<IQueryable<TPH_Poly_TopLevel>, IQueryable<TPH_Poly_TopLevel>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<TPH_Poly_TopLevel>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}