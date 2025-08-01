using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.NotSchema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.NotSchema
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IViewFolderRepository : IEFRepository<ViewFolder, ViewFolder>
    {
        [IntentManaged(Mode.Fully)]
        Task<ViewFolder?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ViewFolder?> FindByIdAsync(Guid id, Func<IQueryable<ViewFolder>, IQueryable<ViewFolder>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ViewFolder>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}