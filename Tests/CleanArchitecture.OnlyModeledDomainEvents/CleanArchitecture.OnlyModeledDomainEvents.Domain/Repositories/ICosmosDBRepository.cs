using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.OnlyModeledDomainEvents.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.OnlyModeledDomainEvents.Domain.Repositories
{
    public interface ICosmosDBRepository<TDomain, TDocumentInterface> : IRepository<TDomain>
    {
        ICosmosDBUnitOfWork UnitOfWork { get; }
        Task<TDomain?> FindAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task<TDomain?> FindAsync(Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int pageSize, Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>>? queryOptions = default, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>>? queryOptions = default, CancellationToken cancellationToken = default);
    }
}