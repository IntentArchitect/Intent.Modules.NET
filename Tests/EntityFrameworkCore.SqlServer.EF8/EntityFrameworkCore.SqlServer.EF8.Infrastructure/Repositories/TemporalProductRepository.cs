using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Common;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities;
using EntityFrameworkCore.SqlServer.EF8.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF8.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TemporalProductRepository : RepositoryBase<TemporalProduct, TemporalProduct, ApplicationDbContext>, ITemporalProductRepository
    {
        public TemporalProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TemporalProduct?> FindByIdAsync(Guid id1, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id1 == id1, cancellationToken);
        }

        public async Task<List<TemporalProduct>> FindByIdsAsync(Guid[] id1s, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = id1s.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id1), cancellationToken);
        }

        public async Task<List<TemporalHistory<TemporalProduct>>> FindHistoryAsync(
            TemporalHistoryQueryOptions historyOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindHistoryAsync<TemporalProduct>(historyOptions, null, null, "StartDate", "EndDate", cancellationToken);
        }

        public async Task<List<TemporalHistory<TemporalProduct>>> FindHistoryAsync(
            TemporalHistoryQueryOptions historyOptions,
            Expression<Func<TemporalProduct, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await FindHistoryAsync<TemporalProduct>(historyOptions, filterExpression, null, "StartDate", "EndDate", cancellationToken);
        }

        public async Task<List<TemporalHistory<TemporalProduct>>> FindHistoryAsync(
            TemporalHistoryQueryOptions historyOptions,
            Expression<Func<TemporalProduct, bool>> filterExpression,
            Func<IQueryable<TemporalProduct>, IQueryable<TemporalProduct>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindHistoryAsync<TemporalProduct>(historyOptions, filterExpression, queryOptions, "StartDate", "EndDate", cancellationToken);
        }
    }
}