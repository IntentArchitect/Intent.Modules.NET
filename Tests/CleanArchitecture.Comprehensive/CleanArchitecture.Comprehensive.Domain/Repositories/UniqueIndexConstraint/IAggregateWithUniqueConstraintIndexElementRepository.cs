using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAggregateWithUniqueConstraintIndexElementRepository : IEFRepository<AggregateWithUniqueConstraintIndexElement, AggregateWithUniqueConstraintIndexElement>
    {
        [IntentManaged(Mode.Fully)]
        Task<AggregateWithUniqueConstraintIndexElement?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AggregateWithUniqueConstraintIndexElement>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}