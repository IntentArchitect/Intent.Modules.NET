using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories.MappableStoredProcs
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IMockEntityRepository : IEFRepository<MockEntity, MockEntity>
    {
        Task<MockEntity> GetMockEntityById(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<MockEntity>> GetMockEntities(CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<MockEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<MockEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}