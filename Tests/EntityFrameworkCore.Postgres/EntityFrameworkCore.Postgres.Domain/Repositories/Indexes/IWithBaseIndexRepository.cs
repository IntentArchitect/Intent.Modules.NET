using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IWithBaseIndexRepository : IEFRepository<WithBaseIndex, WithBaseIndex>
    {
        [IntentManaged(Mode.Fully)]
        Task<WithBaseIndex?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<WithBaseIndex?> FindByIdAsync(Guid id, Func<IQueryable<WithBaseIndex>, IQueryable<WithBaseIndex>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<WithBaseIndex>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}