using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoFramework;
using MongoFramework.Linq;
using Subscribe.GooglePubSub.TestApplication.Domain.Common.Interfaces;
using Subscribe.GooglePubSub.TestApplication.Domain.Repositories;
using Subscribe.GooglePubSub.TestApplication.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.Repositories.MongoRepositoryBase", Version = "1.0")]

namespace Subscribe.GooglePubSub.TestApplication.Infrastructure.Repositories
{
    public abstract class MongoRepositoryBase<TDomain, TPersistence> : IMongoRepository<TDomain, TPersistence>
        where TPersistence : class, TDomain
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
            GetSet().Add((TPersistence)entity);
        }

        public virtual void Remove(TDomain entity)
        {
            GetSet().Remove((TPersistence)entity);
        }

        public virtual void Update(TDomain entity)
        {
            GetSet().Update((TPersistence)entity);
        }

        public virtual List<TDomain> SearchText(
            string searchText,
            Expression<Func<TPersistence, bool>>? filterExpression = null)
        {
            var queryable = GetSet().SearchText(searchText);
            if (filterExpression != null) queryable = queryable.Where(filterExpression);
            return queryable.ToList<TDomain>();
        }

        public virtual async Task<TDomain?> FindAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).SingleOrDefaultAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<TDomain?> FindAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression, linq).SingleOrDefaultAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            return await QueryInternal(x => true).ToListAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).ToListAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression, linq).ToListAsync<TDomain>(cancellationToken);
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
            Expression<Func<TPersistence, bool>> filterExpression,
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
            Expression<Func<TPersistence, bool>> filterExpression,
            int pageNo,
            int pageSize,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq,
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
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).CountAsync(cancellationToken);
        }

        public bool Any(Expression<Func<TPersistence, bool>> filterExpression)
        {
            return QueryInternal(filterExpression).Any();
        }

        public virtual async Task<bool> AnyAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).AnyAsync(cancellationToken);
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
            Func<IQueryable<TPersistence>, IQueryable<TResult>> linq)
        {
            var queryable = CreateQuery();
            queryable = queryable.Where(filterExpression);
            var result = linq(queryable);
            return result;
        }

        protected virtual IQueryable<TPersistence> CreateQuery()
        {
            return GetSet();
        }

        protected virtual IMongoDbSet<TPersistence> GetSet()
        {
            return _dbContext.Set<TPersistence>();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return default;
        }
    }
}