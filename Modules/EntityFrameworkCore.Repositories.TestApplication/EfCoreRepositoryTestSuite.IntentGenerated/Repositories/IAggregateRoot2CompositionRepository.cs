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
    public interface IAggregateRoot2CompositionRepository : IRepository<IAggregateRoot2Composition, AggregateRoot2Composition>
    {
        [IntentManaged(Mode.Fully)]
        Task<IAggregateRoot2Composition> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IAggregateRoot2Composition>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}