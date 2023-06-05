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
    public interface IAggregateRoot3NullableRepository : IEFRepository<AggregateRoot3Nullable, AggregateRoot3Nullable>
    {

        [IntentManaged(Mode.Fully)]
        Task<AggregateRoot3Nullable> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AggregateRoot3Nullable>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}