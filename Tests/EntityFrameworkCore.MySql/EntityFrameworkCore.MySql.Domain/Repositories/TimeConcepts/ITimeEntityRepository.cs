using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.TimeConcepts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.TimeConcepts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITimeEntityRepository : IEFRepository<TimeEntity, TimeEntity>
    {
        [IntentManaged(Mode.Fully)]
        Task<TimeEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TimeEntity>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}