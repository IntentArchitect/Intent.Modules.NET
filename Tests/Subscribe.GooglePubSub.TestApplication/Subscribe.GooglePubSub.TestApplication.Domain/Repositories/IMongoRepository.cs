using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Subscribe.GooglePubSub.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.Repositories.MongoRepositoryInterface", Version = "1.0")]

namespace Subscribe.GooglePubSub.TestApplication.Domain.Repositories
{
    public interface IMongoRepository<TDomain, TPersistence> : IRepository<TDomain>
    {
        IUnitOfWork UnitOfWork { get; }
        Task<TDomain?> FindAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<TDomain?> FindAsync(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    }
}