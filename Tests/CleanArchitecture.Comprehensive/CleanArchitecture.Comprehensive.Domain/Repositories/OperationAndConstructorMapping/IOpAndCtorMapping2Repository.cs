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
    public interface IOpAndCtorMapping2Repository : IEFRepository<OpAndCtorMapping2, OpAndCtorMapping2>
    {
        [IntentManaged(Mode.Fully)]
        Task<OpAndCtorMapping2?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<OpAndCtorMapping2>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}