using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories
{
    public interface ICosmosDBRepository<TDomain, TDocumentInterface> : IRepository<TDomain>
    {
        ICosmosDBUnitOfWork UnitOfWork { get; }
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindAllAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default);
        Task<List<TDomain>> FindByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}