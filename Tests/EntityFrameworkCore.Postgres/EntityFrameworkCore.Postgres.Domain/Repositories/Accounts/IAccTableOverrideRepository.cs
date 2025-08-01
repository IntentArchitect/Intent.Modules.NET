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
    public interface IAccTableOverrideRepository : IEFRepository<AccTableOverride, AccTableOverride>
    {
        [IntentManaged(Mode.Fully)]
        Task<AccTableOverride?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AccTableOverride?> FindByIdAsync(Guid id, Func<IQueryable<AccTableOverride>, IQueryable<AccTableOverride>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AccTableOverride>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}