using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IB_OptionalAggregateRepository : IEFRepository<B_OptionalAggregate, B_OptionalAggregate>
    {
        [IntentManaged(Mode.Fully)]
        Task<B_OptionalAggregate?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<B_OptionalAggregate>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}