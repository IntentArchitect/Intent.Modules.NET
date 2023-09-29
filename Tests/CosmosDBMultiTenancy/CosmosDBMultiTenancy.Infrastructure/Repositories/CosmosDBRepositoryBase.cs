using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    internal abstract class CosmosDBRepositoryBase<TDomain, TPersistence, TDocument> : ICosmosDBRepository<TDomain, TPersistence>
        where TPersistence : TDomain
        where TDocument : TPersistence, ICosmosDBDocument<TDocument, TDomain>, new()
    {
        private readonly CosmosDBUnitOfWork _unitOfWork;
        private readonly Microsoft.Azure.CosmosRepository.IRepository<TDocument> _cosmosRepository;
        private readonly string _idFieldName;
        private readonly string? _tenantId;
        private readonly string _partitionKeyFieldName;
        private readonly Lazy<(string UserName, DateTimeOffset TimeStamp)> _auditDetails;
        private readonly ICurrentUserService _currentUserService;

        protected CosmosDBRepositoryBase(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<TDocument> cosmosRepository,
            string idFieldName,
            string partitionKeyFieldName,
            IMultiTenantContextAccessor<TenantInfo> multiTenantContextAccessor,
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
            var results = documents.Cast<TDomain>().ToList();
            Track(results);

            return results;
        }

        public async Task<TDomain?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var document = await _cosmosRepository.GetAsync(id, _tenantId, cancellationToken: cancellationToken);
            Track(document);

            return document;
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            var predicate = AdaptFilterPredicate(filterExpression);
            predicate = GetFilteredByTenantIdPredicate(predicate);

            var documents = await _cosmosRepository.GetAsync(predicate, cancellationToken);
            var results = documents.Cast<TDomain>().ToList();
            Track(results);

            return results;
        }

        public virtual async Task<IPagedResult<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(_ => true, pageNo, pageSize, cancellationToken);
        }

        public virtual async Task<IPagedResult<TDomain>> FindAllAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var predicate = AdaptFilterPredicate(filterExpression);
            predicate = GetFilteredByTenantIdPredicate(predicate);

            var pagedDocuments = await _cosmosRepository.PageAsync(predicate, pageNo, pageSize, true, cancellationToken);
            Track(pagedDocuments.Items.Cast<TDomain>());

            return new CosmosPagedList<TDomain, TDocument>(pagedDocuments, pageNo, pageSize);
        }

        public async Task<List<TDomain>> FindByIdsAsync(
            IEnumerable<string> ids,
            CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($"SELECT * from c WHERE  (@tenantId = null OR c.{_partitionKeyFieldName} = @tenantId) AND ARRAY_CONTAINS(@ids, c.{_idFieldName})")
                    .WithParameter("@tenantId", _tenantId)
                    .WithParameter("@ids", ids);
            var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);
            var results = documents.Cast<TDomain>().ToList();
            Track(results);

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
        /// Adapts a <typeparamref name="TPersistence"/> predicate to a <typeparamref name="TDocument"/> predicate.
        /// </summary>
        private static Expression<Func<TDocument, bool>> AdaptFilterPredicate(Expression<Func<TPersistence, bool>> expression)
        {
            if (!typeof(TPersistence).IsAssignableFrom(typeof(TDocument))) throw new Exception($"{typeof(TPersistence)} is not assignable from {typeof(TDocument)}.");
            var beforeParameter = expression.Parameters.Single();
            var afterParameter = Expression.Parameter(typeof(TDocument), beforeParameter.Name);
            var visitor = new SubstitutionExpressionVisitor(beforeParameter, afterParameter);
            return Expression.Lambda<Func<TDocument, bool>>(visitor.Visit(expression.Body)!, afterParameter);
        }

        public void Track(IEnumerable<TDomain> items)
        {
            foreach (var item in items)
            {
                _unitOfWork.Track(item);
            }
        }

        public void Track(TDomain item)
        {
            _unitOfWork.Track(item);
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