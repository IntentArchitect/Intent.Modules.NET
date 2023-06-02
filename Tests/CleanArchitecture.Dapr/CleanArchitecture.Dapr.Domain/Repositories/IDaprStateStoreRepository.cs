using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Dapr.Domain.Repositories
{
    public interface IDaprStateStoreRepository<TDomain, TPersistence> : IRepository<TDomain>
    {
        IDaprStateStoreUnitOfWork UnitOfWork { get; }
        Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
    }
}