using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAggregateRoot3AggCollectionRepository : IEFRepository<AggregateRoot3AggCollection, AggregateRoot3AggCollection>
    {

        [IntentManaged(Mode.Fully)]
        Task<AggregateRoot3AggCollection> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AggregateRoot3AggCollection>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}