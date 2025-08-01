using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Entities.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FastEndpointsTest.Domain.Repositories.UniqueIndexConstraint
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAggregateWithUniqueConstraintIndexElementRepository : IEFRepository<AggregateWithUniqueConstraintIndexElement, AggregateWithUniqueConstraintIndexElement>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AggregateWithUniqueConstraintIndexElement?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AggregateWithUniqueConstraintIndexElement?> FindByIdAsync(Guid id, Func<IQueryable<AggregateWithUniqueConstraintIndexElement>, IQueryable<AggregateWithUniqueConstraintIndexElement>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AggregateWithUniqueConstraintIndexElement>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}