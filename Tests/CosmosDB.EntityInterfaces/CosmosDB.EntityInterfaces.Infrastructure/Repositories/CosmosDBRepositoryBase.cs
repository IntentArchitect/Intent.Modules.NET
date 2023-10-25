using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Application.Common.Interfaces;
using CosmosDB.EntityInterfaces.Domain.Common.Interfaces;
using CosmosDB.EntityInterfaces.Domain.Repositories;
using CosmosDB.EntityInterfaces.Infrastructure.Persistence;
using CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepositoryBase", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Repositories
{
    internal abstract class CosmosDBRepositoryBase<TDomain, TDomainState, TDocument, TDocumentInterface> : ICosmosDBRepository<TDomain, TDocumentInterface>
        where TDomain : class
        where TDomainState : class, TDomain
        where TDocument : ICosmosDBDocument<TDomain, TDomainState, TDocument>, TDocumentInterface, new()
    {
        private readonly CosmosDBUnitOfWork _unitOfWork;
        private readonly Microsoft.Azure.CosmosRepository.IRepository<TDocument> _cosmosRepository;
        private readonly string _idFieldName;
        private readonly Lazy<(string UserName, DateTimeOffset TimeStamp)> _auditDetails;
        private readonly ICurrentUserService _currentUserService;

        protected CosmosDBRepositoryBase(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<TDocument> cosmosRepository,
            string idFieldName,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _cosmosRepository = cosmosRepository;
            _idFieldName = idFieldName;
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
                await _cosmosRepository.CreateAsync(document, cancellationToken: cancellationToken);
            });
        }

        public void Update(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                (entity as IAuditable)?.SetUpdated(_auditDetails.Value.UserName, _auditDetails.Value.TimeStamp);
                var document = new TDocument().PopulateFromEntity(entity);
                await _cosmosRepository.UpdateAsync(document, cancellationToken: cancellationToken);
            });
        }

        public void Remove(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity);
                await _cosmosRepository.DeleteAsync(document, cancellationToken: cancellationToken);
            });
        }

        public async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var documents = await _cosmosRepository.GetAsync(_ => true, cancellationToken);
            var results = documents.Select<TDocument, TDomain>(document => document.ToEntity()).ToList();
            Track(results);

            return results;
        }

        public async Task<TDomain?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var document = await _cosmosRepository.GetAsync(id, cancellationToken: cancellationToken);
                var entity = document.ToEntity();
                Track(entity);

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
            var documents = await _cosmosRepository.GetAsync(AdaptFilterPredicate(filterExpression), cancellationToken);
            var results = documents.Select<TDocument, TDomain>(document => document.ToEntity()).ToList();
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
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var pagedDocuments = await _cosmosRepository.PageAsync(AdaptFilterPredicate(filterExpression), pageNo, pageSize, true, cancellationToken);
            Track(pagedDocuments.Items.Select(document => document.ToEntity()));

            return new CosmosPagedList<TDomain, TDomainState, TDocument>(pagedDocuments, pageNo, pageSize);
        }

        public async Task<List<TDomain>> FindByIdsAsync(
            IEnumerable<string> ids,
            CancellationToken cancellationToken = default)
        {
            var queryDefinition = new QueryDefinition($"SELECT * from c WHERE ARRAY_CONTAINS(@ids, c.{_idFieldName})")
                .WithParameter("@ids", ids);
            var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);
            var results = documents.Select<TDocument, TDomain>(document => document.ToEntity()).ToList();
            Track(results);

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