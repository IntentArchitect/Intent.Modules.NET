using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITablePlainRepository : IEFRepository<TablePlain, TablePlain>
    {
        [IntentManaged(Mode.Fully)]
        Task<TablePlain?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TablePlain?> FindByIdAsync(Guid id, Func<IQueryable<TablePlain>, IQueryable<TablePlain>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TablePlain>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}