using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.ExplicitKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPK_PrimaryKeyLongRepository : IEFRepository<PK_PrimaryKeyLong, PK_PrimaryKeyLong>
    {
        [IntentManaged(Mode.Fully)]
        Task<PK_PrimaryKeyLong?> FindByIdAsync(long primaryKeyLong, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<PK_PrimaryKeyLong>> FindByIdsAsync(long[] primaryKeyLongs, CancellationToken cancellationToken = default);
    }
}