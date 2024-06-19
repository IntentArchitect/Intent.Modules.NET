using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MaintainColumnOrder.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IWithKeyConfigRepository : IEFRepository<WithKeyConfig, WithKeyConfig>
    {
        [IntentManaged(Mode.Fully)]
        Task<WithKeyConfig?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<WithKeyConfig>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}