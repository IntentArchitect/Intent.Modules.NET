using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Redis.OM;
using Redis.Om.Repositories.Domain.Common.Interfaces;
using Redis.Om.Repositories.Domain.Repositories;
using Redis.Om.Repositories.Infrastructure.Persistence;
using Redis.Om.Repositories.Infrastructure.Persistence.Documents;
using Redis.OM.Searching;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmRepositoryBase", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Repositories
{
    internal abstract class RedisOmRepositoryBase<TDomain, TDocument, TDocumentInterface> : IRedisOmRepository<TDomain, TDocumentInterface>
        where TDomain : class
        where TDocument : IRedisOmDocument<TDomain, TDocument>, TDocumentInterface, new()
    {
        private readonly RedisOmUnitOfWork _unitOfWork;
        private readonly RedisConnectionProvider _connectionProvider;
        private readonly RedisCollection<TDocument> _collection;
        private readonly string? _collectionName;

        protected RedisOmRepositoryBase(RedisOmUnitOfWork unitOfWork, RedisConnectionProvider connectionProvider)
        {
            _unitOfWork = unitOfWork;
            _connectionProvider = connectionProvider;
            _collection = (RedisCollection<TDocument>)connectionProvider.RedisCollection<TDocument>(500);
            _collectionName = _collection.StateManager.DocumentAttribute.Prefixes.FirstOrDefault()
                              ?? throw new Exception($"{typeof(TDocument).FullName} does not have a Document Prefix assigned.");
        }

        public IRedisOmUnitOfWork UnitOfWork => _unitOfWork;

        public void Add(TDomain entity)
        {
            _unitOfWork.Track(entity);
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity);
                await _collection.InsertAsync(document);
                SetIdValue(entity, document);
            });
        }

        public void Update(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var document = new TDocument().PopulateFromEntity(entity);
                await _collection.UpdateAsync(document);
            });
        }

        public void Remove(TDomain entity)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                await _connectionProvider.Connection.UnlinkAsync($"{_collectionName}:{GetIdValue(entity)}");
            });
        }

        public async Task<TDomain?> FindAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            var document = await _collection.Where(AdaptFilterPredicate(filterExpression)).FirstOrDefaultAsync();
            if (document is null)
            {
                return null;
            }

            var entity = document.ToEntity();
            Track(entity);

            return entity;
        }

        public async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var documents = await _collection.ToListAsync();
            var results = documents.Select(document => document.ToEntity()).ToList();
            Track(results);

            return results;
        }

        public async Task<TDomain?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var document = await _collection.FindByIdAsync(id);

            if (document is null)
            {
                return null;
            }

            var entity = document.ToEntity();
            Track(entity);

            return entity;
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            var documents = await _collection.Where(AdaptFilterPredicate(filterExpression)).ToListAsync();
            var results = documents.Select(document => document.ToEntity()).ToList();
            Track(results);

            return results;
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _collection;
            var count = await query.CountAsync().ConfigureAwait(false);
            var skip = ((pageNo - 1) * pageSize);

            var pagedDocuments = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            var results = pagedDocuments.Select(document => document.ToEntity()).ToList();
            Track(results);

            return new RedisOmPagedList<TDomain, TDocument>(count, pageNo, pageSize, results);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TDocumentInterface, bool>> filterExpression,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _collection.Where(AdaptFilterPredicate(filterExpression));
            var count = await query.CountAsync().ConfigureAwait(false);
            var skip = ((pageNo - 1) * pageSize);

            var pagedDocuments = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            var results = pagedDocuments.Select(document => document.ToEntity()).ToList();
            Track(results);

            return new RedisOmPagedList<TDomain, TDocument>(count, pageNo, pageSize, results);
        }

        public async Task<List<TDomain>> FindByIdsAsync(
            IEnumerable<string> ids,
            CancellationToken cancellationToken = default)
        {
            var documents = await _collection.FindByIdsAsync(ids);

            var results = documents
                .Where(p => p.Value is not null)
                .Select(document => document.Value!.ToEntity())
                .ToList();
            return results;
        }

        protected abstract string GetIdValue(TDomain entity);

        protected abstract void SetIdValue(TDomain domainEntity, TDocument document);

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
                if (node == _before)
                {
                    return _after;
                }

                if (node?.NodeType == ExpressionType.MemberAccess &&
    node is MemberExpression mem &&
    mem.Member.ReflectedType == _before.Type)
                {
                    var newExpression = Visit(mem.Expression);
                    return Expression.MakeMemberAccess(newExpression, _after.Type.GetMember(mem.Member.Name, BindingFlags.Instance | BindingFlags.Public).First());
                }

                return base.Visit(node);
            }
        }
    }
}