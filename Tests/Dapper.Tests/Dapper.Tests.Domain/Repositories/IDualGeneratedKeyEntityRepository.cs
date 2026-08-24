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
    public interface IDualGeneratedKeyEntityRepository : IDapperRepository<DualGeneratedKeyEntity>
    {
        Task UpdateAsync(DualGeneratedKeyEntity entity, CancellationToken cancellationToken = default);
        Task RemoveAsync(DualGeneratedKeyEntity entity, CancellationToken cancellationToken = default);
        Task<DualGeneratedKeyEntity?> FindByIdAsync((Guid KeyPartA, Guid KeyPartB) id, CancellationToken cancellationToken = default);
    }
}