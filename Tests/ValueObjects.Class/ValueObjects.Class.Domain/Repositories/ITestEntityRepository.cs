using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Class.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace ValueObjects.Class.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITestEntityRepository : IEFRepository<TestEntity, TestEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<TestEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TestEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}