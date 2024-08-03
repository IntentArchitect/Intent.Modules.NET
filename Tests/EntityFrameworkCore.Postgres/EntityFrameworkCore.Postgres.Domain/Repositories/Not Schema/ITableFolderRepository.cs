using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.NotSchema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.NotSchema
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITableFolderRepository : IEFRepository<TableFolder, TableFolder>
    {
        [IntentManaged(Mode.Fully)]
        Task<TableFolder?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TableFolder>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}