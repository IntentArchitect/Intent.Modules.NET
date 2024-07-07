using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MultipleDocumentStores.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.Repositories.MongoRepositoryInterface", Version = "1.0")]

namespace MultipleDocumentStores.Domain.Repositories
{
    public interface IMongoRepository<TDomain> : IRepository<TDomain>
    {
        IUnitOfWork UnitOfWork { get; }
        Task<TDomain?> FindAsync(Expression<Func<TDomain, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<TDomain?> FindAsync(Expression<Func<TDomain, bool>> filterExpression, Func<IQueryable<TDomain>, IQueryable<TDomain>> linq, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Expression<Func<TDomain, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Expression<Func<TDomain, bool>> filterExpression, Func<IQueryable<TDomain>, IQueryable<TDomain>> linq, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TDomain, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TDomain, bool>> filterExpression, int pageNo, int pageSize, Func<IQueryable<TDomain>, IQueryable<TDomain>> linq, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TDomain, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TDomain, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<TDomain?> FindAsync(Func<IQueryable<TDomain>, IQueryable<TDomain>> queryOptions, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Func<IQueryable<TDomain>, IQueryable<TDomain>> queryOptions, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int pageSize, Func<IQueryable<TDomain>, IQueryable<TDomain>> queryOptions, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Func<IQueryable<TDomain>, IQueryable<TDomain>>? queryOptions = default, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Func<IQueryable<TDomain>, IQueryable<TDomain>>? queryOptions = default, CancellationToken cancellationToken = default);
    }
}