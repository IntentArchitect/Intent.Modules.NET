using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.SoftDelete;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.SoftDelete
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IChildOfTableParentRepository : IEFRepository<ChildOfTableParent, ChildOfTableParent>
    {
        [IntentManaged(Mode.Fully)]
        Task<ChildOfTableParent?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ChildOfTableParent?> FindByIdAsync(Guid id, Func<IQueryable<ChildOfTableParent>, IQueryable<ChildOfTableParent>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ChildOfTableParent>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}