using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Domain.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using CleanArchitecture.SingleFiles.Infrastructure.Persistence;
using CleanArchitecture.SingleFiles.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepositoryBase", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Infrastructure.Repositories
{
    public static class QueryableAdapter
    {
        public static Func<IQueryable<TDocument>, IQueryable<TDocument>> AdaptQueryFunction<TDocumentInterface, TDocument>(Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions)
            where TDocument : class, TDocumentInterface
        {
            return sourceQueryable =>
                        {
                            // Create a fake queryable of the interface type
                            var interfaceQueryable = new InterfaceQueryableAdapter<TDocumentInterface, TDocument>(sourceQueryable);

                            // Apply the user's query function
                            var resultQueryable = queryOptions(interfaceQueryable);

                            // Extract the adapted queryable
                            if (resultQueryable is InterfaceQueryableAdapter<TDocumentInterface, TDocument> adapter)
                            {
                                return adapter.UnderlyingQueryable;
                            }

                            throw new InvalidOperationException("Query function returned an unexpected queryable type");
                        };
        }
    }
    internal abstract class MongoRepositoryBase<TDomain, TDocument, TDocumentInterface, TIdentifier> : IMongoRepository<TDomain, TDocumentInterface, TIdentifier>
        where TDomain : class
        where TDocument : class, IMongoDbDocument<TDomain, TDocument, TIdentifier>, TDocumentInterface, new()
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
            var result = QueryInternalTDocument(TDocument.GetIdFilterPredicate(id)).SingleOrDefault();

            if (result == null)
            {
                return null;
            }
            return LoadAndTrackDocument(result);
        }

        public virtual async Task<List<TDomain>> FindByIdsAsync(
            TIdentifier[] ids,
            CancellationToken cancellationToken = default)
        {
            var result = QueryInternalTDocument(TDocument.GetIdsFilterPredicate(ids));
            return LoadAndTrackDocuments(result).ToList();
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
            return await MongoPagedList<TDomain, TDocument, TIdentifier>.CreateAsync(query, pageNo, pageSize, cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            int pageNo,
            int pageSize,
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> linq,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression, linq);
            return await MongoPagedList<TDomain, TDocument, TIdentifier>.CreateAsync(query, pageNo, pageSize, cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression);
            return await MongoPagedList<TDomain, TDocument, TIdentifier>.CreateAsync(query, pageNo, pageSize, cancellationToken);
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
            return await MongoPagedList<TDomain, TDocument, TIdentifier>.CreateAsync(query, pageNo, pageSize, cancellationToken);
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

            if (filterExpression != null)
            {
                return QueryInternalTDocument(AdaptFilterPredicate(filterExpression));
            }

            return QueryInternalTDocument(null);
        }

        protected virtual IQueryable<TDocument> QueryInternal(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> linq)
        {
            var queryable = QueryInternal(filterExpression);
            var adaptedQueryFunction = QueryableAdapter.AdaptQueryFunction<TDocumentInterface, TDocument>(linq);
            var result = adaptedQueryFunction(queryable);

            return result;
        }

        protected virtual IQueryable<TDocument> QueryInternalTDocument(Expression<Func<TDocument, bool>>? filterExpression)
        {
            var queryable = _collection.AsQueryable();

            if (filterExpression != null)
            {
                queryable = queryable.Where(filterExpression);
            }

            return queryable;
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

    internal class InterfaceQueryableAdapter<TInterface, TDocument> : IQueryable<TInterface>
        where TDocument : class, TInterface
    {
        private readonly QueryableMethodAdapter _methodAdapter;
        private readonly IQueryable<TDocument> _underlyingQueryable;

        public InterfaceQueryableAdapter(IQueryable<TDocument> underlyingQueryable)
        {
            _underlyingQueryable = underlyingQueryable;
            _methodAdapter = new QueryableMethodAdapter(typeof(TInterface), typeof(TDocument));
        }

        public IQueryable<TDocument> UnderlyingQueryable => _underlyingQueryable;
        public Type ElementType => typeof(TInterface);
        public Expression Expression => _methodAdapter.AdaptExpression(_underlyingQueryable.Expression);
        public IQueryProvider Provider => new InterfaceQueryProvider<TInterface, TDocument>(_underlyingQueryable.Provider, _methodAdapter);

        public IEnumerator<TInterface> GetEnumerator()
        {
            return _underlyingQueryable.Cast<TInterface>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class InterfaceQueryProvider<TInterface, TDocument> : IQueryProvider
        where TDocument : class, TInterface
    {
        private readonly IQueryProvider _underlyingProvider;
        private readonly QueryableMethodAdapter _methodAdapter;

        public InterfaceQueryProvider(IQueryProvider underlyingProvider, QueryableMethodAdapter methodAdapter)
        {
            _underlyingProvider = underlyingProvider;
            _methodAdapter = methodAdapter;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var adaptedExpression = _methodAdapter.AdaptExpressionFromInterface(expression);
            var result = _underlyingProvider.CreateQuery(adaptedExpression);

            if (result is IQueryable<TDocument> typedResult)
            {
                return new InterfaceQueryableAdapter<TInterface, TDocument>(typedResult);
            }
            return result;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == typeof(TInterface))
            {
                var adaptedExpression = _methodAdapter.AdaptExpressionFromInterface(expression);
                var result = _underlyingProvider.CreateQuery<TDocument>(adaptedExpression);
                return (IQueryable<TElement>)(object)new InterfaceQueryableAdapter<TInterface, TDocument>(result);
            }
            var directAdaptedExpression = _methodAdapter.AdaptExpressionFromInterface(expression);
            return _underlyingProvider.CreateQuery<TElement>(directAdaptedExpression);
        }

        public object Execute(Expression expression)
        {
            var adaptedExpression = _methodAdapter.AdaptExpressionFromInterface(expression);
            return _underlyingProvider.Execute(adaptedExpression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var adaptedExpression = _methodAdapter.AdaptExpressionFromInterface(expression);
            return _underlyingProvider.Execute<TResult>(adaptedExpression);
        }
    }

    internal class QueryableMethodAdapter
    {
        private readonly Type _interfaceType;
        private readonly Type _documentType;

        public QueryableMethodAdapter(Type interfaceType, Type documentType)
        {
            _interfaceType = interfaceType;
            _documentType = documentType;
        }

        public Expression AdaptExpression(Expression expression)
        {
            return new TypeSubstitutionVisitor(_documentType, _interfaceType).Visit(expression);
        }

        public Expression AdaptExpressionFromInterface(Expression expression)
        {
            return new TypeSubstitutionVisitor(_interfaceType, _documentType).Visit(expression);
        }
    }

    internal class TypeSubstitutionVisitor : ExpressionVisitor
    {
        private readonly Type _fromType;
        private readonly Type _toType;

        public TypeSubstitutionVisitor(Type fromType, Type toType)
        {
            _fromType = fromType;
            _toType = toType;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == _fromType)
            {
                return Expression.Parameter(_toType, node.Name);
            }
            return base.VisitParameter(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.IsGenericMethod)
            {
                var genericArgs = node.Method.GetGenericArguments();
                var newGenericArgs = new Type[genericArgs.Length];
                bool hasChanges = false;

                for (int i = 0; i < genericArgs.Length; i++)
                {
                    if (genericArgs[i] == _fromType)
                    {
                        newGenericArgs[i] = _toType;
                        hasChanges = true;
                    }
                    else
                    {
                        newGenericArgs[i] = genericArgs[i];
                    }
                }

                if (hasChanges)
                {
                    var newMethod = node.Method.GetGenericMethodDefinition().MakeGenericMethod(newGenericArgs);
                    var newObject = Visit(node.Object);
                    var newArgs = node.Arguments.Select(Visit).ToArray();
                    return Expression.Call(newObject, newMethod, newArgs);
                }
            }
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var newParameters = node.Parameters.Select(p =>
                                        p.Type == _fromType ? Expression.Parameter(_toType, p.Name) : p).ToArray();

            if (newParameters.SequenceEqual(node.Parameters))
            {
                return base.VisitLambda(node);
            }

            var parameterMap = node.Parameters.Zip(newParameters, (old, @new) => new { old, @new })
                .ToDictionary(x => x.old, x => x.@new);

            var visitor = new ParameterReplacementVisitor(parameterMap);
            var newBody = visitor.Visit(node.Body);

            // Create new lambda with correct delegate type
            var delegateType = typeof(T);
            if (delegateType.IsGenericType)
            {
                var genericArgs = delegateType.GetGenericArguments();
                var newGenericArgs = genericArgs.Select(arg => arg == _fromType ? _toType : arg).ToArray();

                if (!newGenericArgs.SequenceEqual(genericArgs))
                {
                    var genericDefinition = delegateType.GetGenericTypeDefinition();
                    delegateType = genericDefinition.MakeGenericType(newGenericArgs);
                }
            }

            return Expression.Lambda(delegateType, newBody, newParameters);
        }
    }

    internal class ParameterReplacementVisitor : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _parameterMap;

        public ParameterReplacementVisitor(Dictionary<ParameterExpression, ParameterExpression> parameterMap)
        {
            _parameterMap = parameterMap;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _parameterMap.TryGetValue(node, out var replacement) ? replacement : node;
        }
    }
}