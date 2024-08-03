using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITableOverrideRepository : IEFRepository<TableOverride, TableOverride>
    {
        [IntentManaged(Mode.Fully)]
        Task<TableOverride?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TableOverride>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}