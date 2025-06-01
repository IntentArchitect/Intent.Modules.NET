using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using RichDomain.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace RichDomain.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IBaseClassRepository : IEFRepository<IBaseClass, BaseClass>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<IBaseClass?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<IBaseClass?> FindByIdAsync(Guid id, Func<IQueryable<BaseClass>, IQueryable<BaseClass>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IBaseClass>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}