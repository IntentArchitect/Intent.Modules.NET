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
    public class B_OptionalDependentRepository : RepositoryBase<B_OptionalDependent, B_OptionalDependent, ApplicationDbContext>, IB_OptionalDependentRepository
    {
        public B_OptionalDependentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<B_OptionalDependent?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<B_OptionalDependent?> FindByIdAsync(
            Guid id,
            Func<IQueryable<B_OptionalDependent>, IQueryable<B_OptionalDependent>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<B_OptionalDependent>> FindByIdsAsync(
            Guid[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}