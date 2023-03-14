using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Repository;
using Subscribe.GooglePubSub.TestApplication.Domain.Common.Interfaces;
using Subscribe.GooglePubSub.TestApplication.Domain.Repositories;
using Subscribe.GooglePubSub.TestApplication.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.Repositories.MongoRepositoryBase", Version = "1.0")]

namespace Subscribe.GooglePubSub.TestApplication.Infrastructure.Repositories
{
    public abstract class MongoRepositoryBase<TDomain, TPersistence> : MongoDbRepository<TPersistence>, IRepository<TDomain, TPersistence>
        where TPersistence : class, TDomain
        where TDomain : class
    {
        public MongoRepositoryBase(ApplicationMongoDbContext context) : base(context)
        {
            UnitOfWork = context;
        }

        public IUnitOfWork UnitOfWork { get; }

        public virtual void Add(TDomain entity)
        {
            base.InsertOne((TPersistence)entity);
        }

        public abstract void Remove(TDomain entity);

        public virtual object Update(Expression<Func<TPersistence, bool>> predicate, TDomain entity)
        {
            return base.UpdateOne(predicate, (TPersistence)entity, null);
        }

        public virtual Task<TDomain> FindAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default)
        {
            return FindAsync(filterExpression, null, cancellationToken);
        }

        public virtual async Task<TDomain> FindAsync(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq, CancellationToken cancellationToken = default)
        {
            IMongoQueryable<TPersistence> query = Context.GetCollection<TPersistence>().AsQueryable();
            if (filterExpression != null)
            {
                query = query.Where(filterExpression);
            }
            if (linq != null)
            {
                query = (IMongoQueryable<TPersistence>)linq(query);
            }
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            return FindAllAsync(null, cancellationToken);
        }

        public virtual Task<List<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default)
        {
            return FindAllAsync(filterExpression, null, cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq, CancellationToken cancellationToken = default)
        {
            IMongoQueryable<TPersistence> query = Context.GetCollection<TPersistence>().AsQueryable();
            if (filterExpression != null)
            {
                query = query.Where(filterExpression);
            }
            if (linq != null)
            {
                query = (IMongoQueryable<TPersistence>)linq(query);
            }
            return (await query.ToListAsync(cancellationToken)).Cast<TDomain>().ToList();
        }

        public virtual Task<IPagedResult<TDomain>> FindAllAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default)
        {
            return FindAllAsync(null, pageNo, pageSize, cancellationToken);
        }

        public virtual Task<IPagedResult<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default)
        {
            return FindAllAsync(filterExpression, pageNo, pageSize, null, cancellationToken);
        }

        public virtual async Task<IPagedResult<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq, CancellationToken cancellationToken = default)
        {
            IMongoQueryable<TPersistence> query = Context.GetCollection<TPersistence>().AsQueryable();
            if (filterExpression != null)
            {
                query = query.Where(filterExpression);
            }
            if (linq != null)
            {
                query = (IMongoQueryable<TPersistence>)linq(query);
            }
            return await PagedList<TPersistence>.CreateAsync(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }
    }
}