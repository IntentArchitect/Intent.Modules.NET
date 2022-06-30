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
    public interface IAggregateRoot5EntityWithRepoRepository : IRepository<IAggregateRoot5EntityWithRepo, AggregateRoot5EntityWithRepo>
    {
        [IntentManaged(Mode.Fully)]
        Task<IAggregateRoot5EntityWithRepo> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IAggregateRoot5EntityWithRepo>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}