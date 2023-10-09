using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Domain.Repositories
{
    public interface ICosmosDBRepository<TDomain, TPersistence> : IRepository<TDomain>
    {
        ICosmosDBUnitOfWork UnitOfWork { get; }
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
        Task<TDomain?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<IPagedResult<TDomain>> FindAllAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<IPagedResult<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}