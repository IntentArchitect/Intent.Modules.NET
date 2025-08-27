using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MultipleDocumentStores.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepositoryInterface", Version = "1.0")]

namespace MultipleDocumentStores.Domain.Repositories
{
    public interface IMongoRepository<TDomain, TDocumentInterface, TIdentifier> : IRepository<TDomain>
    {
        IMongoDbUnitOfWork UnitOfWork { get; }
        Task<TDomain?> FindAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<TDomain?> FindAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> linq, CancellationToken cancellationToken = default);
        Task<TDomain?> FindAsync(Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> linq, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, int pageNo, int pageSize, Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> linq, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int pageSize, Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>>? queryOptions = default, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>>? queryOptions = default, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<TDomain> FindByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindByIdsAsync(TIdentifier[] ids, CancellationToken cancellationToken = default);
        List<TDomain> SearchText(string searchText, Expression<Func<TDocumentInterface, bool>>? filterExpression = null);
    }
}