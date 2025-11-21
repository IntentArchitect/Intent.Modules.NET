using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF10.Domain.Entities.TPT.Polymorphic;
using EntityFrameworkCore.SqlServer.EF10.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF10.Domain.Repositories.TPT.Polymorphic;
using EntityFrameworkCore.SqlServer.EF10.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Infrastructure.Repositories.TPT.Polymorphic
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPT_Poly_ConcreteARepository : RepositoryBase<TPT_Poly_ConcreteA, TPT_Poly_ConcreteA, ApplicationDbContext>, ITPT_Poly_ConcreteARepository
    {
        public TPT_Poly_ConcreteARepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPT_Poly_ConcreteA?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<TPT_Poly_ConcreteA?> FindByIdAsync(
            Guid id,
            Func<IQueryable<TPT_Poly_ConcreteA>, IQueryable<TPT_Poly_ConcreteA>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<TPT_Poly_ConcreteA>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}