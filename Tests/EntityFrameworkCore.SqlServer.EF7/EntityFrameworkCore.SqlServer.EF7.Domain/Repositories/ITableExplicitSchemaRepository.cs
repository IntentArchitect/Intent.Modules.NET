using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF7.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITableExplicitSchemaRepository : IEFRepository<TableExplicitSchema, TableExplicitSchema>
    {
        [IntentManaged(Mode.Fully)]
        Task<TableExplicitSchema?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TableExplicitSchema?> FindByIdAsync(Guid id, Func<IQueryable<TableExplicitSchema>, IQueryable<TableExplicitSchema>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TableExplicitSchema>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}