using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Repositories.TPH.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Infrastructure.Repositories.TPH.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPH_DerivedClassForConcreteRepository : RepositoryBase<TPH_DerivedClassForConcrete, TPH_DerivedClassForConcrete, ApplicationDbContext>, ITPH_DerivedClassForConcreteRepository
    {
        public TPH_DerivedClassForConcreteRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TPH_DerivedClassForConcrete?> FindByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<TPH_DerivedClassForConcrete?> FindByIdAsync(
            Guid id,
            Func<IQueryable<TPH_DerivedClassForConcrete>, IQueryable<TPH_DerivedClassForConcrete>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<TPH_DerivedClassForConcrete>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}