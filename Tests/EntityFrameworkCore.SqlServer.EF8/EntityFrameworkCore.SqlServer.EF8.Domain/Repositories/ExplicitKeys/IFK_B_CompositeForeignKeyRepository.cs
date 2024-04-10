using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Repositories.ExplicitKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IFK_B_CompositeForeignKeyRepository : IEFRepository<FK_B_CompositeForeignKey, FK_B_CompositeForeignKey>
    {
        [IntentManaged(Mode.Fully)]
        Task<FK_B_CompositeForeignKey?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<FK_B_CompositeForeignKey>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}