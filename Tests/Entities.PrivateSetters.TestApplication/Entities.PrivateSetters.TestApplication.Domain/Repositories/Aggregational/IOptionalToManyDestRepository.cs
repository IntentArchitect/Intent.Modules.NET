using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IOptionalToManyDestRepository : IEFRepository<OptionalToManyDest, OptionalToManyDest>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<OptionalToManyDest?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<OptionalToManyDest?> FindByIdAsync(Guid id, Func<IQueryable<OptionalToManyDest>, IQueryable<OptionalToManyDest>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<OptionalToManyDest>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}