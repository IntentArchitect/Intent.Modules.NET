using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Application.Common.Pagination;
using TableStorage.Tests.Domain.Common.Interfaces;
using TableStorage.Tests.Domain.Repositories;
using TableStorage.Tests.Infrastructure.Persistence;
using TableStorage.Tests.Infrastructure.Persistence.Tables;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.TableStorage.TableStorageRepositoryBase", Version = "1.0")]

namespace TableStorage.Tests.Infrastructure.Repositories
{
    internal abstract class TableStorageRepositoryBase<TDomain, TTable, TTableInterface> : ITableStorageRepository<TDomain, TTableInterface>
        where TDomain : class
        where TTable : class, ITableAdapter<TDomain, TTable>, new()
    {
        private readonly TableStorageUnitOfWork _unitOfWork;
        private readonly TableClient _tableClient;

        protected TableStorageRepositoryBase(TableStorageUnitOfWork unitOfWork,
            TableServiceClient tableServiceClient,
            string tableName)
        {
            _unitOfWork = unitOfWork;
            _tableClient = tableServiceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        public ITableStorageUnitOfWork UnitOfWork => _unitOfWork;

        public void Add(TDomain entity)
        {
            _unitOfWork.Track(entity);
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TTable().PopulateFromEntity(entity);
                await _tableClient.AddEntityAsync(document, cancellationToken: cancellationToken);
            });
        }

        public void Update(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TTable().PopulateFromEntity(entity);
                await _tableClient.UpdateEntityAsync(document, ETag.All, cancellationToken: cancellationToken);
            });
        }

        public void Remove(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TTable().PopulateFromEntity(entity);
                await _tableClient.DeleteEntityAsync(document.PartitionKey, document.RowKey);
            });
        }

        public async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var results = new List<TDomain>();
            var response = _tableClient.QueryAsync<TTable>(cancellationToken: cancellationToken);

            await foreach (var document in response)
            {
                results.Add(document.ToEntity());
            }
            Track(results);

            return results;
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TTableInterface, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            var results = new List<TDomain>();
            var response = _tableClient.QueryAsync(AdaptFilterPredicate(filterExpression), cancellationToken: cancellationToken);

            await foreach (var document in response)
            {
                results.Add(document.ToEntity());
            }
            Track(results);

            return results;
        }

        public async Task<ICursorPagedList<TDomain>> FindAllAsync(
            int pageSize,
            string? cursorToken,
            CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(_ => true, pageSize, cursorToken, cancellationToken);
        }

        public async Task<ICursorPagedList<TDomain>> FindAllAsync(
            Expression<Func<TTableInterface, bool>> filterExpression,
            int pageSize,
            string? cursorToken,
            CancellationToken cancellationToken = default)
        {
            var results = new List<TDomain>();
            var nextCursorToken = string.Empty;
            var response = _tableClient.QueryAsync(AdaptFilterPredicate(filterExpression), cancellationToken: cancellationToken)
                .AsPages(cursorToken, pageSize);

            await foreach (var page in response)
            {
                foreach (var document in page.Values)
                {
                    results.Add(document.ToEntity());
                }
                nextCursorToken = page.ContinuationToken;
                break;
            }
            Track(results);

            return new CursorPagedList<TDomain>(nextCursorToken, pageSize, results);
        }

        public async Task<TDomain?> FindByIdAsync(
            (string partitionKey, string rowKey) id,
            CancellationToken cancellationToken = default)
        {
            var response = await _tableClient.GetEntityAsync<TTable>(id.partitionKey, id.rowKey, cancellationToken: cancellationToken);
            var entity = response.Value.ToEntity();
            Track(entity);

            return entity;
        }

        public async Task<TDomain?> FindAsync(
            Expression<Func<TTableInterface, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            var documents = _tableClient.QueryAsync(AdaptFilterPredicate(filterExpression), cancellationToken: cancellationToken);
            TDomain? entity = null;
            await foreach (var document in documents)
            {
                entity = document.ToEntity();
                break;
            }

            if (entity is null)
            {
                return default;
            }
            Track(entity);

            return entity;
        }

        /// <summary>
        /// Adapts a <typeparamref name="TTableInterface"/> predicate to a <typeparamref name="TTable"/> predicate.
        /// </summary>
        private static Expression<Func<TTable, bool>> AdaptFilterPredicate(Expression<Func<TTableInterface, bool>> expression)
        {
            if (!typeof(TTableInterface).IsAssignableFrom(typeof(TTable))) throw new Exception($"typeof(TTableInterface) is not assignable from typeof(TTable).");
            var beforeParameter = expression.Parameters.Single();
            var afterParameter = Expression.Parameter(typeof(TTable), beforeParameter.Name);
            var visitor = new SubstitutionExpressionVisitor(beforeParameter, afterParameter);
            return Expression.Lambda<Func<TTable, bool>>(visitor.Visit(expression.Body)!, afterParameter);
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