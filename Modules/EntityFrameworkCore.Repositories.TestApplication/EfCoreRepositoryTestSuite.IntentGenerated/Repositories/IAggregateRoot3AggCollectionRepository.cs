using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfCoreRepositoryTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAggregateRoot3AggCollectionRepository : IRepository<IAggregateRoot3AggCollection, AggregateRoot3AggCollection>
    {
        [IntentManaged(Mode.Fully)]
        Task<IAggregateRoot3AggCollection> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IAggregateRoot3AggCollection>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}