using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ID_OptionalAggregateRepository : IEFRepository<D_OptionalAggregate, D_OptionalAggregate>
    {
        [IntentManaged(Mode.Fully)]
        Task<D_OptionalAggregate?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<D_OptionalAggregate?> FindByIdAsync(Guid id, Func<IQueryable<D_OptionalAggregate>, IQueryable<D_OptionalAggregate>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<D_OptionalAggregate>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}