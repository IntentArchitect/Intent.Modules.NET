using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FastEndpointsTest.Domain.Repositories.CRUD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAggregateRootLongRepository : IEFRepository<AggregateRootLong, AggregateRootLong>
    {
        [IntentManaged(Mode.Fully)]
        Task<AggregateRootLong?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AggregateRootLong>> FindByIdsAsync(long[] ids, CancellationToken cancellationToken = default);
    }
}