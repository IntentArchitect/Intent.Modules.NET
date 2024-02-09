using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CosmosDBMultiTenancy.Application.Common.Interfaces;
using CosmosDBMultiTenancy.Domain.Common.Interfaces;
using CosmosDBMultiTenancy.Domain.Repositories;
using CosmosDBMultiTenancy.Infrastructure.Persistence;
using CosmosDBMultiTenancy.Infrastructure.Persistence.Documents;
using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepositoryBase", Version = "1.0")]

namespace CosmosDBMultiTenancy.Infrastructure.Repositories
{
    internal abstract class CosmosDBRepositoryBase<TDomain, TDocument, TDocumentInterface> : ICosmosDBRepository<TDomain, TDocumentInterface>
        where TDomain : class
        where TDocument : ICosmosDBDocument<TDomain, TDocument>, TDocumentInterface, new()
    {
        private readonly CosmosDBUnitOfWork _unitOfWork;
        private readonly Microsoft.Azure.CosmosRepository.IRepository<TDocument> _cosmosRepository;
        private readonly string _idFieldName;
        private readonly string? _tenantId;
        private readonly string? _partitionKeyFieldName;
        private readonly Lazy<(string UserName, DateTimeOffset TimeStamp)> _auditDetails;
        private readonly ICurrentUserService _currentUserService;

        protected CosmosDBRepositoryBase(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<TDocument> cosmosRepository,
            string idFieldName,
            string? partitionKeyFieldName,
            IMultiTenantContextAccessor<TenantInfo>? multiTenantContextAccessor,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _cosmosRepository = cosmosRepository;
            _idFieldName = idFieldName;
            _partitionKeyFieldName = partitionKeyFieldName;

            if (multiTenantContextAccessor != null)
            {
                _tenantId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id ?? throw new InvalidOperationException("Could not resolve tenantId");
            }
            _currentUserService = currentUserService;
            _auditDetails = new Lazy<(string UserName, DateTimeOffset TimeStamp)>(GetAuditDetails);
        }

        public ICosmosDBUnitOfWork UnitOfWork => _unitOfWork;

        public void Add(TDomain entity)
        {
            _unitOfWork.Track(entity);
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                (entity as IAuditable)?.SetCreated(_auditDetails.Value.UserName, _auditDetails.Value.TimeStamp);
                var document = new TDocument().PopulateFromEntity(entity);

                CheckTenancy(document);

                await _cosmosRepository.CreateAsync(document, cancellationToken: cancellationToken);
            });
        }

        public void Update(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                (entity as IAuditable)?.SetUpdated(_auditDetails.Value.UserName, _auditDetails.Value.TimeStamp);
                var document = new TDocument().PopulateFromEntity(entity);

                CheckTenancy(document);

                await _cosmosRepository.UpdateAsync(document, cancellationToken: cancellationToken);
            });
        }

        public void Remove(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity);

                CheckTenancy(document);

                await _cosmosRepository.DeleteAsync(document, cancellationToken: cancellationToken);
            });
        }

        public async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var documents = await _cosmosRepository.GetAsync(GetHasPartitionKeyExpression(_tenantId), cancellationToken);
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
                var document = await _cosmosRepository.GetAsync(id, partitionKey ?? _tenantId, cancellationToken: cancellationToken);
                var entity = LoadAndTrackDocument(document);

                return entity;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            var predicate = AdaptFilterPredicate(filterExpression);
            predicate = GetFilteredByTenantIdPredicate(predicate);

            var documents = await _cosmosRepository.GetAsync(predicate, cancellationToken);
            var results = LoadAndTrackDocuments(documents).ToList();

            return results;
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(_ => true, pageNo, pageSize, cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var predicate = AdaptFilterPredicate(filterExpression);
            predicate = GetFilteredByTenantIdPredicate(predicate);

            var pagedDocuments = await _cosmosRepository.PageAsync(predicate, pageNo, pageSize, true, cancellationToken);
            var entities = LoadAndTrackDocuments(pagedDocuments.Items).ToList();

            var totalCount = pagedDocuments.Total ?? 0;
            var pageCount = pagedDocuments.TotalPages ?? 0;

            return new CosmosPagedList<TDomain, TDocument>(entities, totalCount, pageCount, pageNo, pageSize);
        }

        public async Task<List<TDomain>> FindByIdsAsync(
            IEnumerable<string> ids,
            CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($"SELECT * from c WHERE {(!string.IsNullOrEmpty(_partitionKeyFieldName) ? "(@tenantId = null OR c.{_partitionKeyFieldName} = @tenantId)  AND " : "")}ARRAY_CONTAINS(@ids, c.{_idFieldName})")
                    .WithParameter("@tenantId", _tenantId)
                    .WithParameter("@ids", ids);
            var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);
            var results = LoadAndTrackDocuments(documents).ToList();

            return results;
        }

        public void CheckTenancy(TDocument document)
        {
            if (_tenantId == null)
            {
                return;
            }

            document.PartitionKey ??= _tenantId;
            if (document.PartitionKey != _tenantId)
            {
                throw new InvalidOperationException("TenantId mismatch");
            }
        }

        /// <summary>
        /// Returns a predicate which filters by tenantId in addition to the provided <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">The existing predicate to also filter by.</param>
        private Expression<Func<TDocument, bool>> GetFilteredByTenantIdPredicate(Expression<Func<TDocument, bool>> predicate)
        {
            if (_tenantId == null) return predicate;
            var restrictToTenantPredicate = GetHasPartitionKeyExpression(_tenantId);
            return Expression.Lambda<Func<TDocument, bool>>(Expression.AndAlso(predicate.Body, restrictToTenantPredicate.Body), restrictToTenantPredicate.Parameters[0]);
        }

        protected virtual Expression<Func<TDocument, bool>> GetHasPartitionKeyExpression(string? partitionKey)
        {
            return _ => true;
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

            return entity;
        }

        public IEnumerable<TDomain> LoadAndTrackDocuments(IEnumerable<TDocument> documents)
        {
            foreach (var document in documents)
            {
                yield return LoadAndTrackDocument(document);
            }
        }

        public string? GetEtag(TDomain entity)
        {
            if (_etags.TryGetValue(GetId(entity), out var etag))
            {
                return etag;
            }

            return default;
        }

        private (string UserName, DateTimeOffset TimeStamp) GetAuditDetails()
        {
            var userName = _currentUserService.UserId ?? throw new InvalidOperationException("UserId is null");
            var timestamp = DateTimeOffset.UtcNow;

            return (userName, timestamp);
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