using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BasicAuditing.CustomUserId.Tests.Application.Common.Interfaces;
using BasicAuditing.CustomUserId.Tests.Domain.Common.Interfaces;
using BasicAuditing.CustomUserId.Tests.Domain.Repositories;
using BasicAuditing.CustomUserId.Tests.Infrastructure.Persistence;
using BasicAuditing.CustomUserId.Tests.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepositoryBase", Version = "1.0")]

namespace BasicAuditing.CustomUserId.Tests.Infrastructure.Repositories
{
    internal abstract class CosmosDBRepositoryBase<TDomain, TDocument, TDocumentInterface> : ICosmosDBRepository<TDomain, TDocumentInterface>
        where TDomain : class
        where TDocument : ICosmosDBDocument<TDomain, TDocument>, TDocumentInterface, new()
    {
        private readonly Dictionary<string, string?> _eTags = new Dictionary<string, string?>();
        private readonly string _documentType;
        private readonly CosmosDBUnitOfWork _unitOfWork;
        private readonly Microsoft.Azure.CosmosRepository.IRepository<TDocument> _cosmosRepository;
        private readonly string _idFieldName;
        private readonly ICosmosContainerProvider<TDocument> _containerProvider;
        private readonly IOptionsMonitor<RepositoryOptions> _optionsMonitor;
        private readonly Lazy<(Guid UserIdentifier, DateTimeOffset TimeStamp)> _auditDetails;
        private readonly ICurrentUserService _currentUserService;

        protected CosmosDBRepositoryBase(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<TDocument> cosmosRepository,
            string idFieldName,
            ICosmosContainerProvider<TDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _cosmosRepository = cosmosRepository;
            _idFieldName = idFieldName;
            _containerProvider = containerProvider;
            _optionsMonitor = optionsMonitor;
            _documentType = typeof(TDocument).GetNameForDocument();
            _currentUserService = currentUserService;
            _auditDetails = new Lazy<(Guid UserIdentifier, DateTimeOffset TimeStamp)>(GetAuditDetails);
        }

        public ICosmosDBUnitOfWork UnitOfWork => _unitOfWork;

        public void Add(TDomain entity)
        {
            _unitOfWork.Track(entity);
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                (entity as IAuditable)?.SetCreated(_auditDetails.Value.UserIdentifier, _auditDetails.Value.TimeStamp);
                var document = new TDocument().PopulateFromEntity(entity, _ => null);
                await _cosmosRepository.CreateAsync(document, cancellationToken: cancellationToken);
            });
        }

        public void Update(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                (entity as IAuditable)?.SetUpdated(_auditDetails.Value.UserIdentifier, _auditDetails.Value.TimeStamp);
                var document = new TDocument().PopulateFromEntity(entity, _eTags.GetValueOrDefault);
                await _cosmosRepository.UpdateAsync(document, cancellationToken: cancellationToken);
            });
        }

        public void Remove(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity, _eTags.GetValueOrDefault);
                await _cosmosRepository.DeleteAsync(document, cancellationToken: cancellationToken);
            });
        }

        public async Task<TDomain?> FindAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            var documents = await _cosmosRepository.GetAsync(AdaptFilterPredicate(filterExpression), cancellationToken).ToListAsync();

            if (!documents.Any())
            {
                return default;
            }
            var entity = LoadAndTrackDocument(documents.First());

            return entity;
        }

        public async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var documents = await _cosmosRepository.GetAsync(_ => true, cancellationToken);
            var results = LoadAndTrackDocuments(documents).ToList();

            return results;
        }

        protected async Task<TDomain?> FindByIdAsync(
            string id,
            string? partitionKey = default,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var document = await _cosmosRepository.GetAsync(id, partitionKey, cancellationToken: cancellationToken);
                var entity = LoadAndTrackDocument(document);

                return entity;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            var documents = await _cosmosRepository.GetAsync(AdaptFilterPredicate(filterExpression), cancellationToken);
            var results = LoadAndTrackDocuments(documents).ToList();

            return results;
        }

        public async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(_ => true, pageNo, pageSize, cancellationToken);
        }

        public async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var pagedDocuments = await _cosmosRepository.PageAsync(AdaptFilterPredicate(filterExpression), pageNo, pageSize, true, cancellationToken);
            var entities = LoadAndTrackDocuments(pagedDocuments.Items).ToList();

            var totalCount = pagedDocuments.Total ?? 0;
            var pageCount = pagedDocuments.TotalPages ?? 0;

            return new CosmosPagedList<TDomain, TDocument>(entities, totalCount, pageCount, pageNo, pageSize);
        }

        public async Task<List<TDomain>> FindByIdsAsync(
            IEnumerable<string> ids,
            CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($"SELECT * from c WHERE ARRAY_CONTAINS(@ids, c.{_idFieldName})")
                .WithParameter("@ids", ids);

            return await FindAllAsync(queryDefinition);
        }

        public async Task<TDomain?> FindAsync(
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var queryable = await CreateQuery(queryOptions);
            var documents = await ProcessResults(queryable, cancellationToken);

            if (!documents.Any())
            {
                return default;
            }
            var entity = LoadAndTrackDocument(documents.First());

            return entity;
        }

        public async Task<List<TDomain>> FindAllAsync(
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var queryable = await CreateQuery(queryOptions);
            var documents = await ProcessResults(queryable, cancellationToken);
            var results = LoadAndTrackDocuments(documents).ToList();

            return results;
        }

        public async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var queryable = await CreateQuery(queryOptions, new QueryRequestOptions() { MaxItemCount = pageSize });
            var countResponse = await queryable.CountAsync(cancellationToken);
            queryable = queryable
                    .Skip(pageSize * (pageNo - 1))
                    .Take(pageSize);

            var documents = await ProcessResults(queryable, cancellationToken);
            var entities = LoadAndTrackDocuments(documents).ToList();

            var totalCount = countResponse ?? 0;
            var pageCount = (int)Math.Abs(Math.Ceiling(totalCount / (double)pageSize));

            return new CosmosPagedList<TDomain, TDocument>(entities, totalCount, pageCount, pageNo, pageSize);
        }

        public async Task<int> CountAsync(
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>>? queryOptions = default,
            CancellationToken cancellationToken = default)
        {
            var queryable = await CreateQuery(queryOptions);

            return await queryable.CountAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>>? queryOptions = default,
            CancellationToken cancellationToken = default)
        {
            var queryable = await CreateQuery(queryOptions);

            return await queryable.CountAsync(cancellationToken) > 0;
        }

        protected async Task<List<TDomain>> FindAllAsync(
            QueryDefinition queryDefinition,
            CancellationToken cancellationToken = default)
        {
            var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);
            var results = LoadAndTrackDocuments(documents).ToList();

            return results;
        }

        protected async Task<TDomain?> FindAsync(
            QueryDefinition queryDefinition,
            CancellationToken cancellationToken = default)
        {
            var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);

            if (!documents.Any())
            {
                return default;
            }
            var entity = LoadAndTrackDocument(documents.First());

            return entity;
        }

        protected async Task<IQueryable<TDocumentInterface>> CreateQuery(
            Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>>? queryOptions = default,
            QueryRequestOptions? requestOptions = default)
        {
            var container = await _containerProvider.GetContainerAsync();
            var queryable = (IQueryable<TDocumentInterface>)container.GetItemLinqQueryable<TDocumentInterface>(requestOptions: requestOptions, linqSerializerOptions: _optionsMonitor.CurrentValue.SerializationOptions);
            queryable = queryOptions == null ? queryable : queryOptions(queryable);
            //Filter by document type
            queryable = queryable.Where(d => ((IItem)d!).Type == _documentType);

            return queryable;
        }

        protected async Task<List<TProjection>> ProcessResults<TProjection>(
            IQueryable<TProjection> query,
            CancellationToken cancellationToken = default)
        {
            var results = new List<TProjection>();

            using var feedIterator = query.ToFeedIterator();

            while (feedIterator.HasMoreResults)
            {
                results.AddRange(await feedIterator.ReadNextAsync(cancellationToken));
            }

            return results;
        }

        protected async Task<List<TDocument>> ProcessResults(
            IQueryable<TDocumentInterface> query,
            CancellationToken cancellationToken = default)
        {
            var results = new List<TDocument>();

            using var feedIterator = query.Select(x => (TDocument)x).ToFeedIterator();

            while (feedIterator.HasMoreResults)
            {
                results.AddRange(await feedIterator.ReadNextAsync(cancellationToken));
            }

            return results;
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

        public TDomain LoadAndTrackDocument(TDocument document)
        {
            var entity = document.ToEntity();

            _unitOfWork.Track(entity);
            _eTags[document.Id] = document.Etag;

            return entity;
        }

        public IEnumerable<TDomain> LoadAndTrackDocuments(IEnumerable<TDocument> documents)
        {
            foreach (var document in documents)
            {
                yield return LoadAndTrackDocument(document);
            }
        }

        private (Guid UserIdentifier, DateTimeOffset TimeStamp) GetAuditDetails()
        {
            var userIdentifier = _currentUserService.UserId ?? throw new InvalidOperationException("UserId is null");
            var timestamp = DateTimeOffset.UtcNow;

            return (userIdentifier, timestamp);
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