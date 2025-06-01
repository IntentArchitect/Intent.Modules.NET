using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Entities;
using EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Repositories;
using EntityFrameworkCore.MaintainColumnOrder.Tests.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.MaintainColumnOrder.Tests.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BaseWithLastRepository : RepositoryBase<BaseWithLast, BaseWithLast, ApplicationDbContext>, IBaseWithLastRepository
    {
        public BaseWithLastRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<BaseWithLast?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<BaseWithLast?> FindByIdAsync(
            Guid id,
            Func<IQueryable<BaseWithLast>, IQueryable<BaseWithLast>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<BaseWithLast>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}