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
    public interface IAggregateRoot4AggNullableRepository : IEFRepository<AggregateRoot4AggNullable, AggregateRoot4AggNullable>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);

        [IntentManaged(Mode.Fully)]
        Task<AggregateRoot4AggNullable?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AggregateRoot4AggNullable?> FindByIdAsync(Guid id, Func<IQueryable<AggregateRoot4AggNullable>, IQueryable<AggregateRoot4AggNullable>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AggregateRoot4AggNullable>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}