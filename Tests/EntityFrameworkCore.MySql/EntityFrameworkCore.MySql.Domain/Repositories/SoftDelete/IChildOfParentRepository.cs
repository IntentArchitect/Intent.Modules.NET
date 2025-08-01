using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.SoftDelete;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.SoftDelete
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IChildOfParentRepository : IEFRepository<ChildOfParent, ChildOfParent>
    {
        [IntentManaged(Mode.Fully)]
        Task<ChildOfParent?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ChildOfParent?> FindByIdAsync(Guid id, Func<IQueryable<ChildOfParent>, IQueryable<ChildOfParent>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ChildOfParent>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}