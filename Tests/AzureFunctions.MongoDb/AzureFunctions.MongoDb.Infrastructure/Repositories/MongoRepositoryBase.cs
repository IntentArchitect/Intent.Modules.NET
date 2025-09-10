using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Common.Interfaces;
using AzureFunctions.MongoDb.Domain.Repositories;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepositoryBase", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories
{
    internal abstract class MongoRepositoryBase<TDomain, TIdentifier> : IMongoRepository<TDomain, TIdentifier>
        where TDomain : class
    {
        private readonly Func<TDomain, TIdentifier> _getId;
        private readonly IMongoCollection<TDomain> _collection;
        private readonly MongoDbUnitOfWork _unitOfWork;
        private readonly Expression<Func<TDomain, TIdentifier>> _idSelector;

        protected MongoRepositoryBase(IMongoCollection<TDomain> collection,
            MongoDbUnitOfWork unitOfWork,
            Expression<Func<TDomain, TIdentifier>> idSelector)
        {
            _collection = collection;
            _unitOfWork = unitOfWork;
            _idSelector = idSelector;
            _getId = idSelector.Compile();
        }

        public IMongoDbUnitOfWork UnitOfWork => _unitOfWork;

        public virtual void Add(TDomain entity)
        {
            _unitOfWork.Track(entity);
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            });
        }

        public virtual void Remove(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                await _collection.DeleteOneAsync(GetIdFilter(entity), cancellationToken: cancellationToken);
            });
        }

        public virtual void Update(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                await _collection.ReplaceOneAsync(GetIdFilter(entity), entity, cancellationToken: cancellationToken);
            });
        }

        public virtual List<TDomain> SearchText(
            string searchText,
            Expression<Func<TDomain, bool>>? filterExpression = null)
        {
            var textFilter = Builders<TDomain>.Filter.Text(searchText);
            FilterDefinition<TDomain> combinedFilter = textFilter;

            if (filterExpression != null)
            {
                var adaptedFilter = Builders<TDomain>.Filter.Where(filterExpression);
                combinedFilter = Builders<TDomain>.Filter.And(textFilter, adaptedFilter);
            }
            var documents = _collection.Find(combinedFilter).ToList();
            return documents.Select(LoadAndTrackDocument).ToList();
        }

        public virtual async Task<TDomain?> FindAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            var documents = QueryInternal(filterExpression);

            if (!documents.Any())
            {
                return default;
            }
            var entity = LoadAndTrackDocument(documents.First());

            return entity;
        }

        public virtual async Task<TDomain?> FindAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            Func<IQueryable<TDomain>, IQueryable<TDomain>> linq,
            CancellationToken cancellationToken = default)
        {
            var documents = QueryInternal(filterExpression, linq);
            var document = await documents.FirstOrDefaultAsync(cancellationToken);

            if (document == null)
            {
                return default;
            }
            return LoadAndTrackDocument(document);
        }

        public virtual async Task<TDomain?> FindAsync(
            Func<IQueryable<TDomain>, IQueryable<TDomain>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var documents = QueryInternal(x => true, queryOptions);
            var document = await documents.FirstOrDefaultAsync(cancellationToken);

            if (document == null)
            {
                return default;
            }

            return LoadAndTrackDocument(document);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var documents = QueryInternal(x => true);

            return LoadAndTrackDocuments(documents).ToList();
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            var documents = QueryInternal(filterExpression);

            if (!documents.Any())
            {
                return default;
            }

            return LoadAndTrackDocuments(documents).ToList();
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            Func<IQueryable<TDomain>, IQueryable<TDomain>> linq,
            CancellationToken cancellationToken = default)
        {
            var documents = QueryInternal(filterExpression, linq);

            if (documents == null)
            {
                return default;
            }

            return LoadAndTrackDocuments(documents).ToList();
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(x => true);
            return await MongoPagedList<TDomain, TIdentifier>.CreateAsync(query, pageNo, pageSize, cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            int pageNo,
            int pageSize,
            Func<IQueryable<TDomain>, IQueryable<TDomain>> linq,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression, linq);
            return await MongoPagedList<TDomain, TIdentifier>.CreateAsync(query, pageNo, pageSize, cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression);
            return await MongoPagedList<TDomain, TIdentifier>.CreateAsync(query, pageNo, pageSize, cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Func<IQueryable<TDomain>, IQueryable<TDomain>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(x => true, queryOptions);
            var documents = await query.ToListAsync(cancellationToken);

            return LoadAndTrackDocuments(documents).ToList();
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            Func<IQueryable<TDomain>, IQueryable<TDomain>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(x => true, queryOptions);
            return await MongoPagedList<TDomain, TIdentifier>.CreateAsync(query, pageNo, pageSize, cancellationToken);
        }

        public virtual async Task<int> CountAsync(
            Expression<Func<TDomain, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).CountAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(
            Func<IQueryable<TDomain>, IQueryable<TDomain>>? queryOptions = default,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(x => true, queryOptions).CountAsync(cancellationToken);
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

        public virtual async Task<bool> AnyAsync(
            Func<IQueryable<TDomain>, IQueryable<TDomain>>? queryOptions = default,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(x => true, queryOptions).AnyAsync(cancellationToken);
        }

        public TDomain LoadAndTrackDocument(TDomain entity)
        {

            _unitOfWork.Track(entity);

            return entity;
        }

        public IEnumerable<TDomain> LoadAndTrackDocuments(IEnumerable<TDomain> entities)
        {
            foreach (var entity in entities)
            {
                yield return LoadAndTrackDocument(entity);
            }
        }

        protected FilterDefinition<TDomain> GetIdFilter(TDomain entity) => Builders<TDomain>.Filter.Eq(_idSelector, _getId(entity));

        protected virtual IQueryable<TDomain> QueryInternal(Expression<Func<TDomain, bool>>? filterExpression)
        {
            if (filterExpression != null)
            {
                return QueryInternalTDocument(filterExpression);
            }

            return QueryInternalTDocument(null);
        }

        protected virtual IQueryable<TDomain> QueryInternal(
            Expression<Func<TDomain, bool>> filterExpression,
            Func<IQueryable<TDomain>, IQueryable<TDomain>> linq)
        {
            var queryable = QueryInternal(filterExpression);
            var result = linq(queryable);

            return result;
        }

        protected virtual IQueryable<TDomain> QueryInternalTDocument(Expression<Func<TDomain, bool>>? filterExpression)
        {
            var queryable = _collection.AsQueryable();

            if (filterExpression != null)
            {
                queryable = queryable.Where(filterExpression);
            }

            return queryable;
        }
    }
}