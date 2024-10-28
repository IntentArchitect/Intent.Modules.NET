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
    public interface IAggregateTestNoIdReturnRepository : IEFRepository<AggregateTestNoIdReturn, AggregateTestNoIdReturn>
    {
        [IntentManaged(Mode.Fully)]
        Task<AggregateTestNoIdReturn?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AggregateTestNoIdReturn>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}