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
    public interface IPK_B_CompositeKeyRepository : IEFRepository<PK_B_CompositeKey, PK_B_CompositeKey>
    {
        [IntentManaged(Mode.Fully)]
        Task<PK_B_CompositeKey?> FindByIdAsync((Guid CompositeKeyA, Guid CompositeKeyB) id, CancellationToken cancellationToken = default);
    }
}