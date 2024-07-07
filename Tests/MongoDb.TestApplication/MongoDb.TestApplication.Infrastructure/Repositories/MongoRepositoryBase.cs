using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDb.TestApplication.Domain.Common.Interfaces;
using MongoDb.TestApplication.Domain.Repositories;
using MongoDb.TestApplication.Infrastructure.Persistence;
using MongoFramework;
using MongoFramework.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.Repositories.MongoRepositoryBase", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Repositories
{
    public abstract class MongoRepositoryBase<TDomain> : IMongoRepository<TDomain>
        where TDomain : class
    {
        private readonly ApplicationMongoDbContext _dbContext;
        protected MongoRepositoryBase(ApplicationMongoDbContext context)
        {
            _dbContext = context;
            UnitOfWork = context;
        }

        public IUnitOfWork UnitOfWork { get; }

        public virtual void Add(TDomain entity)
        {
            GetSet().Add((TDomain)entity);
        }

        public virtual void Remove(TDomain entity)
        {
            GetSet().Remove((TDomain)entity);
        }

        public virtual void Update(TDomain entity)
        {
            GetSet().Update((TDomain)entity);
        }

        public virtual List<TDomain> SearchText(
            string searchText,
            Expression<Func<TDomain, bool>>? filterExpression = null)
        {
            var queryable = GetSet().SearchText(searchText);
            if (filterExpression != null) queryable = queryable.Where(filterExpression);
            return queryable.ToList();
        }

        public virtual async Task<TDomain?> FindAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).SingleOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<TDomain?> FindAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            Func<IQueryable<TDomain>, IQueryable<TDomain>> linq,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression, linq).SingleOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            return await QueryInternal(x => true).ToListAsync(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).ToListAsync(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            Func<IQueryable<TDomain>, IQueryable<TDomain>> linq,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression, linq).ToListAsync(cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(x => true);
            return await MongoPagedList<TDomain>.CreateAsync(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression);
            return await MongoPagedList<TDomain>.CreateAsync(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            int pageNo,
            int pageSize,
            Func<IQueryable<TDomain>, IQueryable<TDomain>> linq,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression, linq);
            return await MongoPagedList<TDomain>.CreateAsync(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public virtual async Task<int> CountAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).CountAsync(cancellationToken);
        }

        public bool Any(Expression<Func<TDomain, bool>> filterExpression)
        {
            return QueryInternal(filterExpression).Any();
        }

        public virtual async Task<bool> AnyAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).AnyAsync(cancellationToken);
        }

        public virtual async Task<TDomain?> FindAsync(
            Func<IQueryable<TDomain>, IQueryable<TDomain>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var queryable = CreateQuery();
            queryable = queryOptions(queryable);
            return await queryable.SingleOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Func<IQueryable<TDomain>, IQueryable<TDomain>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var queryable = CreateQuery();
            queryable = queryOptions(queryable);
            return await queryable.ToListAsync(cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            Func<IQueryable<TDomain>, IQueryable<TDomain>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(_ => true, queryOptions);
            return await MongoPagedList<TDomain>.CreateAsync(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public virtual async Task<int> CountAsync(
            Func<IQueryable<TDomain>, IQueryable<TDomain>>? queryOptions = default,
            CancellationToken cancellationToken = default)
        {
            var queryable = CreateQuery();
            queryable = queryOptions == null ? queryable : queryOptions(queryable);
            return await queryable.CountAsync(cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(
            Func<IQueryable<TDomain>, IQueryable<TDomain>>? queryOptions = default,
            CancellationToken cancellationToken = default)
        {
            var queryable = CreateQuery();
            queryable = queryOptions == null ? queryable : queryOptions(queryable);
            return await queryable.AnyAsync(cancellationToken);
        }

        protected virtual IQueryable<TDomain> QueryInternal(Expression<Func<TDomain, bool>>? filterExpression)
        {
            var queryable = CreateQuery();
            if (filterExpression != null)
            {
                queryable = queryable.Where(filterExpression);
            }
            return queryable;
        }

        protected virtual IQueryable<TResult> QueryInternal<TResult>(
            Expression<Func<TDomain, bool>> filterExpression,
            Func<IQueryable<TDomain>, IQueryable<TResult>> linq)
        {
            var queryable = CreateQuery();
            queryable = queryable.Where(filterExpression);
            var result = linq(queryable);
            return result;
        }

        protected virtual IQueryable<TDomain> CreateQuery()
        {
            return GetSet();
        }

        protected virtual IMongoDbSet<TDomain> GetSet()
        {
            return _dbContext.Set<TDomain>();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return default;
        }
    }
}