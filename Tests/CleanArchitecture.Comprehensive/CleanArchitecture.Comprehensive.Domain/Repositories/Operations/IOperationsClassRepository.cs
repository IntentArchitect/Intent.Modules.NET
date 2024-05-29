using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.Operations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.Operations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IOperationsClassRepository : IEFRepository<OperationsClass, OperationsClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<OperationsClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<OperationsClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}