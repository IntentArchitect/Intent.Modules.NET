using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapper.EntityRepositoryInterface", Version = "1.0")]

namespace Dapper.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IMixedKeyEntityRepository : IDapperRepository<MixedKeyEntity>
    {
        Task UpdateAsync(MixedKeyEntity entity, CancellationToken cancellationToken = default);
        Task RemoveAsync(MixedKeyEntity entity, CancellationToken cancellationToken = default);
        Task<MixedKeyEntity?> FindByIdAsync((Guid TenantId, Guid RowId) id, CancellationToken cancellationToken = default);
    }
}