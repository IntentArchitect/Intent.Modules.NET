using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Application.Common.Pagination;
using EntityFrameworkCore.SqlServer.EF8.Domain.Common;
using EntityFrameworkCore.SqlServer.EF8.Domain.Common.Interfaces;
using EntityFrameworkCore.SqlServer.EF8.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.RepositoryBase", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Infrastructure.Repositories
{
    public class RepositoryBase<TDomain, TPersistence, TDbContext> : IEFRepository<TDomain, TPersistence>
        where TDbContext : DbContext, IUnitOfWork
        where TPersistence : class, TDomain
        where TDomain : class
    {
        protected readonly TDbContext _dbContext;

        public RepositoryBase(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IUnitOfWork UnitOfWork => _dbContext;

        public virtual void Remove(TDomain entity)
        {
            GetSet().Remove((TPersistence)entity);
        }

        public virtual void Add(TDomain entity)
        {
            GetSet().Add((TPersistence)entity);
        }

        public virtual void Update(TDomain entity)
        {
            GetSet().Update((TPersistence)entity);
        }

        public virtual async Task<TDomain?> FindAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).SingleOrDefaultAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<TDomain?> FindAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression, queryOptions).SingleOrDefaultAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression: null).ToListAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).ToListAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression, queryOptions).ToListAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression: null);
            return await ToPagedListAsync<TDomain>(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression);
            return await ToPagedListAsync<TDomain>(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            int pageNo,
            int pageSize,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression, queryOptions);
            return await ToPagedListAsync<TDomain>(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public virtual async Task<int> CountAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).CountAsync(cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).AnyAsync(cancellationToken);
        }

        public virtual async Task<TDomain?> FindAsync(
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(queryOptions).SingleOrDefaultAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(queryOptions).ToListAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(queryOptions);
            return await ToPagedListAsync<TDomain>(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public virtual async Task<int> CountAsync(
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>>? queryOptions = default,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(queryOptions).CountAsync(cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>>? queryOptions = default,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(queryOptions).AnyAsync(cancellationToken);
        }

        protected virtual IQueryable<TPersistence> QueryInternal(Expression<Func<TPersistence, bool>>? filterExpression)
        {
            var queryable = CreateQuery();
            if (filterExpression != null)
            {
                queryable = queryable.Where(filterExpression);
            }
            return queryable;
        }

        protected virtual IQueryable<TResult> QueryInternal<TResult>(
            Expression<Func<TPersistence, bool>> filterExpression,
            Func<IQueryable<TPersistence>, IQueryable<TResult>> queryOptions)
        {
            var queryable = CreateQuery();
            queryable = queryable.Where(filterExpression);
            var result = queryOptions(queryable);
            return result;
        }

        protected virtual IQueryable<TPersistence> QueryInternal(Func<IQueryable<TPersistence>, IQueryable<TPersistence>>? queryOptions)
        {
            var queryable = CreateQuery();
            if (queryOptions != null)
            {
                queryable = queryOptions(queryable);
            }
            return queryable;
        }

        protected virtual IQueryable<TPersistence> CreateQuery()
        {
            return GetSet();
        }

        protected virtual DbSet<TPersistence> GetSet()
        {
            return _dbContext.Set<TPersistence>();
        }

        protected async Task<List<TemporalHistory<TDomain>>> FindHistoryAsync<TTemporalPersistence>(
            TemporalHistoryQueryOptions historyOptions,
            string validFromColumnName = "PeriodStart",
            string validToColumnName = "PeriodEnd",
            CancellationToken cancellationToken = default)
            where TTemporalPersistence : class, TDomain, TPersistence, ITemporal
        {
            return await FindHistoryAsync<TTemporalPersistence>(historyOptions, null, null, validFromColumnName, validToColumnName, cancellationToken);
        }

        protected async Task<List<TemporalHistory<TDomain>>> FindHistoryAsync<TTemporalPersistence>(
            TemporalHistoryQueryOptions historyOptions,
            Expression<Func<TTemporalPersistence, bool>> filterExpression,
            string validFromColumnName = "PeriodStart",
            string validToColumnName = "PeriodEnd",
            CancellationToken cancellationToken = default)
            where TTemporalPersistence : class, TDomain, TPersistence, ITemporal
        {
            return await FindHistoryAsync<TTemporalPersistence>(historyOptions, filterExpression, null, validFromColumnName, validToColumnName, cancellationToken);
        }

        protected async Task<List<TemporalHistory<TDomain>>> FindHistoryAsync<TTemporalPersistence>(
            TemporalHistoryQueryOptions historyOptions,
            Expression<Func<TTemporalPersistence, bool>> filterExpression,
            Func<IQueryable<TTemporalPersistence>, IQueryable<TTemporalPersistence>> queryOptions,
            string validFromColumnName = "PeriodStart",
            string validToColumnName = "PeriodEnd",
            CancellationToken cancellationToken = default)
            where TTemporalPersistence : class, TDomain, TPersistence, ITemporal
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
            return await queryable.Select(entity => new TemporalHistory<TDomain>(entity, EF.Property<DateTime>(entity, validFromColumnName), EF.Property<DateTime>(entity, validToColumnName))).ToListAsync(cancellationToken);

            IQueryable<TTemporalPersistence> GetEntityQueryable(TemporalHistoryQueryType queryType)
            {
                switch (queryType)
                {
                    case TemporalHistoryQueryType.All:
                        return dbSet.TemporalAll().Cast<TTemporalPersistence>();
                    case TemporalHistoryQueryType.AsOf:
                        return dbSet.TemporalAsOf(internalDateFrom).Cast<TTemporalPersistence>();
                    case TemporalHistoryQueryType.Between:
                        return dbSet.TemporalBetween(internalDateFrom, internalDateTo).Cast<TTemporalPersistence>();
                    case TemporalHistoryQueryType.ContainedIn:
                        return dbSet.TemporalContainedIn(internalDateFrom, internalDateTo).Cast<TTemporalPersistence>();
                    case TemporalHistoryQueryType.FromTo:
                        return dbSet.TemporalFromTo(internalDateFrom, internalDateTo).Cast<TTemporalPersistence>();
                    default:
                        return dbSet.TemporalBetween(internalDateFrom, internalDateTo).Cast<TTemporalPersistence>();
                }
            }
        }

        private static async Task<IPagedList<T>> ToPagedListAsync<T>(
            IQueryable<T> queryable,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var count = await queryable.CountAsync(cancellationToken);
            var skip = ((pageNo - 1) * pageSize);

            var results = await queryable
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            return new PagedList<T>(count, pageNo, pageSize, results);
        }
    }
}