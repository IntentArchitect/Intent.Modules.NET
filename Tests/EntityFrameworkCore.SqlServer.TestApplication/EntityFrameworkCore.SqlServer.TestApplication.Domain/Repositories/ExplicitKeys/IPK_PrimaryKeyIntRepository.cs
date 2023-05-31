using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.ExplicitKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPK_PrimaryKeyIntRepository : IEfRepository<PK_PrimaryKeyInt, PK_PrimaryKeyInt>
    {

        [IntentManaged(Mode.Fully)]
        Task<PK_PrimaryKeyInt> FindByIdAsync(int primaryKeyId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<PK_PrimaryKeyInt>> FindByIdsAsync(int[] primaryKeyIds, CancellationToken cancellationToken = default);
    }
}