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
    public interface IAggregateWithUniqueConstraintIndexStereotypeRepository : IEFRepository<AggregateWithUniqueConstraintIndexStereotype, AggregateWithUniqueConstraintIndexStereotype>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AggregateWithUniqueConstraintIndexStereotype?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AggregateWithUniqueConstraintIndexStereotype?> FindByIdAsync(Guid id, Func<IQueryable<AggregateWithUniqueConstraintIndexStereotype>, IQueryable<AggregateWithUniqueConstraintIndexStereotype>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AggregateWithUniqueConstraintIndexStereotype>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}