using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.MongoDb.Domain.Common.Interfaces;
using Entities.PrivateSetters.MongoDb.Domain.Repositories;
using Entities.PrivateSetters.MongoDb.Infrastructure.Persistence;
using Entities.PrivateSetters.MongoDb.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepositoryBase", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Infrastructure.Repositories
{
    internal abstract class MongoRepositoryBase<TDomain, TDocument, TDocumentInterface, TIdentifier> : IMongoRepository<TDomain, TDocumentInterface, TIdentifier>
        where TDomain : class
        where TDocument : IMongoDbDocument<TDomain, TDocument, TIdentifier>, TDocumentInterface, new()
    {
        private readonly IMongoCollection<TDocument> _collection;
        private readonly MongoDbUnitOfWork _unitOfWork;

        protected MongoRepositoryBase(IMongoCollection<TDocument> collection, MongoDbUnitOfWork unitOfWork)
        {
            _collection = collection;
            _unitOfWork = unitOfWork;
        }

        public IMongoDbUnitOfWork UnitOfWork => _unitOfWork;

        public virtual void Add(TDomain entity)
        {
            _unitOfWork.Track(entity);
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity);
                await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
                document.ToEntity(entity);
            });
        }

        public virtual void Remove(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity);
                await _collection.DeleteOneAsync(document.GetIdFilter(), cancellationToken: cancellationToken);
            });
        }

        public virtual async Task<TDomain> FindByIdAsync(TIdentifier id, CancellationToken cancellationToken = default)
        {
            var cursor = await _collection.FindAsync(TDocument.GetIdFilter(id));
            return LoadAndTrackDocument(cursor.Single());
        }

        public virtual async Task<List<TDomain>> FindByIdsAsync(
            TIdentifier[] ids,
            CancellationToken cancellationToken = default)
        {
            var cursor = await _collection.FindAsync(TDocument.GetIdsFilter(ids));
            return LoadAndTrackDocuments(cursor.ToEnumerable()).ToList();
        }

        public virtual void Update(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity);
                await _collection.ReplaceOneAsync(document.GetIdFilter(), document, cancellationToken: cancellationToken);
            });
        }

        public virtual List<TDomain> SearchText(
            string searchText,
            Expression<Func<TDocumentInterface, bool>>? filterExpression = null)
        {
            var textFilter = Builders<TDocument>.Filter.Text(searchText);
            FilterDefinition<TDocument> combinedFilter = textFilter;

            if (filterExpression != null)
            {
                var adaptedFilter = Builders<TDocument>.Filter.Where(AdaptFilterPredicate(filterExpression));
                combinedFilter = Builders<TDocument>.Filter.And(textFilter, adaptedFilter);
            }
            var documents = _collection.Find(combinedFilter).ToList();
            return documents.Select(LoadAndTrackDocument).ToList();
        }

        public virtual async Task<TDomain?> FindAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
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
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> linq,
            CancellationToken cancellationToken = default)
        {
            var documents = QueryInternal(filterExpression, linq);
            var document = await documents.FirstOrDefaultAsync(cancellationToken);

            if (document == null)
            {
                return default;
            }
            return LoadAndTrackDocument((TDocument)document);
        }

        public virtual async Task<TDomain?> FindAsync(
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var documents = QueryInternal(x => true, queryOptions);
            var document = await documents.FirstOrDefaultAsync(cancellationToken);

            if (document == null)
            {
                return default;
            }

            return LoadAndTrackDocument((TDocument)document);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var documents = QueryInternal(x => true);

            return LoadAndTrackDocuments(documents).ToList();
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
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
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> linq,
            CancellationToken cancellationToken = default)
        {
            var documents = QueryInternal(filterExpression, linq);

            if (documents == null)
            {
                return default;
            }

            return LoadAndTrackDocuments(documents.Select(d => (TDocument)d)).ToList();
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(x => true);
            return await MongoPagedList<TDomain, TDocument, TIdentifier>.CreateAsync(query.Cast<TDomain>(), pageNo, pageSize, cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            int pageNo,
            int pageSize,
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> linq,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression, linq);
            return await MongoPagedList<TDomain, TDocument, TIdentifier>.CreateAsync(query.Cast<TDomain>(), pageNo, pageSize, cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression);
            return await MongoPagedList<TDomain, TDocument, TIdentifier>.CreateAsync(query.Cast<TDomain>(), pageNo, pageSize, cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(x => true, queryOptions);
            var documents = await query.ToListAsync(cancellationToken);

            return LoadAndTrackDocuments(documents).ToList();
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(x => true, queryOptions);
            return await MongoPagedList<TDomain, TDocument, TIdentifier>.CreateAsync(query.Cast<TDomain>(), pageNo, pageSize, cancellationToken);
        }

        public virtual async Task<int> CountAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).CountAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>>? queryOptions = default,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(x => true, queryOptions).CountAsync(cancellationToken);
        }

        public bool Any(Expression<Func<TDocumentInterface, bool>> filterExpression)
        {
            return QueryInternal(filterExpression).Any();
        }

        public virtual async Task<bool> AnyAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).AnyAsync(cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>>? queryOptions = default,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(x => true, queryOptions).AnyAsync(cancellationToken);
        }

        public TDomain LoadAndTrackDocument(TDocument document)
        {
            var entity = document.ToEntity();

            _unitOfWork.Track(entity);

            return entity;
        }

        public IEnumerable<TDomain> LoadAndTrackDocuments(IEnumerable<TDocument> documents)
        {
            foreach (var document in documents)
            {
                yield return LoadAndTrackDocument(document);
            }
        }

        protected virtual IQueryable<TDocument> QueryInternal(Expression<Func<TDocumentInterface, bool>>? filterExpression)
        {
            var queryable = _collection.AsQueryable();

            if (filterExpression != null)
            {
                queryable = queryable.Where(AdaptFilterPredicate(filterExpression));
            }

            return queryable;
        }

        protected virtual IQueryable<TDocument> QueryInternal(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> linq)
        {
            var queryable = QueryInternal(filterExpression);
            var castable = linq(queryable.Cast<TDocumentInterface>());

            return castable.Cast<TDocument>();
        }

        /// <summary>
        /// Adapts a <typeparamref name="TDocumentInterface"/> predicate to a <typeparamref name="TDocument"/> predicate.
        /// </summary>
        private static Expression<Func<TDocument, bool>> AdaptFilterPredicate(Expression<Func<TDocumentInterface, bool>> expression)
        {
            var beforeParameter = expression.Parameters.Single();
            var afterParameter = Expression.Parameter(typeof(TDocument), beforeParameter.Name);
            var visitor = new SubstitutionExpressionVisitor(beforeParameter, afterParameter);
            return Expression.Lambda<Func<TDocument, bool>>(visitor.Visit(expression.Body)!, afterParameter);
        }

        private class SubstitutionExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _before;
            private readonly Expression _after;

            public SubstitutionExpressionVisitor(Expression before, Expression after)
            {
                _before = before;
                _after = after;
            }

            public override Expression? Visit(Expression? node)
            {
                return node == _before ? _after : base.Visit(node);
            }
        }
    }
}