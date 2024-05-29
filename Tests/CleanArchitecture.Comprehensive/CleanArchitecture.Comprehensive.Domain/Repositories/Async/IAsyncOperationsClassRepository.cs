using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.Async;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.Async
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAsyncOperationsClassRepository : IEFRepository<AsyncOperationsClass, AsyncOperationsClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<AsyncOperationsClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AsyncOperationsClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}