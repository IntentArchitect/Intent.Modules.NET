using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.EdgeCompositeHandling;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.EdgeCompositeHandling
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IExternalRepository : IEFRepository<External, External>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<External?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<External?> FindByIdAsync(Guid id, Func<IQueryable<External>, IQueryable<External>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<External>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}