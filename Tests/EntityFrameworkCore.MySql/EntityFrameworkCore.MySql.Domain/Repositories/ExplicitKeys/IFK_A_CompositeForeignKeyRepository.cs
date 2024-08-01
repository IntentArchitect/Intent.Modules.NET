using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.ExplicitKeys;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.ExplicitKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IFK_A_CompositeForeignKeyRepository : IEFRepository<FK_A_CompositeForeignKey, FK_A_CompositeForeignKey>
    {
        [IntentManaged(Mode.Fully)]
        Task<FK_A_CompositeForeignKey?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<FK_A_CompositeForeignKey>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}