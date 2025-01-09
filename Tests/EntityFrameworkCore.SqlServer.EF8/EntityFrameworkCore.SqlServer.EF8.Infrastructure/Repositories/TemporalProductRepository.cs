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
using Microsoft.EntityFrameworkCore;

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
            return await FindAllAsync(x => id1s.Contains(x.Id1), cancellationToken);
        }

        public async Task<List<TemporalHistory<TemporalProduct>>> FindHistoryAsync(
            TemporalHistoryQueryOptions historyOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindHistoryAsync(historyOptions, null, null, cancellationToken);
        }

        public async Task<List<TemporalHistory<TemporalProduct>>> FindHistoryAsync(
            TemporalHistoryQueryOptions historyOptions,
            Expression<Func<TemporalProduct, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await FindHistoryAsync(historyOptions, filterExpression, null, cancellationToken);
        }

        public async Task<List<TemporalHistory<TemporalProduct>>> FindHistoryAsync(
            TemporalHistoryQueryOptions historyOptions,
            Expression<Func<TemporalProduct, bool>> filterExpression,
            Func<IQueryable<TemporalProduct>, IQueryable<TemporalProduct>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var internalDateFrom = (historyOptions is null || historyOptions.DateFrom == null || historyOptions.DateFrom == DateTime.MinValue) ? DateTime.MinValue : historyOptions!.DateFrom.Value;
            var internalDateTo = (historyOptions is null || historyOptions.DateTo == null || historyOptions.DateTo == DateTime.MinValue) ? DateTime.MinValue : historyOptions!.DateTo.Value;
            var queryType = (historyOptions is null || historyOptions?.QueryType == null) ? TemporalHistoryQueryType.All : historyOptions.QueryType.Value;
            var dbSet = GetSet();
            var queryable = GetEntityQueryable(queryType);

            if (filterExpression != null)
            {
                queryable = queryable.Where(filterExpression);
            }

            if (queryOptions != null)
            {
                queryable = queryOptions(queryable);
            }
            return await queryable.Select(entity => new TemporalHistory<TemporalProduct>(entity, EF.Property<DateTime>(entity, "StartDate"), EF.Property<DateTime>(entity, "EndDate"))).ToListAsync(cancellationToken);

            IQueryable<TemporalProduct> GetEntityQueryable(TemporalHistoryQueryType queryType)
            {
                switch (queryType)
                {
                    case TemporalHistoryQueryType.All:
                        return dbSet.TemporalAll();
                    case TemporalHistoryQueryType.AsOf:
                        return dbSet.TemporalAsOf(internalDateFrom);
                    case TemporalHistoryQueryType.Between:
                        return dbSet.TemporalBetween(internalDateFrom, internalDateTo);
                    case TemporalHistoryQueryType.ContainedIn:
                        return dbSet.TemporalContainedIn(internalDateFrom, internalDateTo);
                    case TemporalHistoryQueryType.FromTo:
                        return dbSet.TemporalFromTo(internalDateFrom, internalDateTo);
                    default:
                        return dbSet.TemporalBetween(internalDateFrom, internalDateTo);
                }
            }
        }
    }
}