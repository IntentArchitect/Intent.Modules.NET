using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.OperationAndConstructorMapping;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.OperationAndConstructorMapping
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IOpAndCtorMapping3Repository : IEFRepository<OpAndCtorMapping3, OpAndCtorMapping3>
    {
        [IntentManaged(Mode.Fully)]
        Task<OpAndCtorMapping3?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<OpAndCtorMapping3>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}