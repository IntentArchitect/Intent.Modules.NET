using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.Accounts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.Accounts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAccViewOverrideRepository : IEFRepository<AccViewOverride, AccViewOverride>
    {
        [IntentManaged(Mode.Fully)]
        Task<AccViewOverride?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AccViewOverride?> FindByIdAsync(Guid id, Func<IQueryable<AccViewOverride>, IQueryable<AccViewOverride>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AccViewOverride>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}