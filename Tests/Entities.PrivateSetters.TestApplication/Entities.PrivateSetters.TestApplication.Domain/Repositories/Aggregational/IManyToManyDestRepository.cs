using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IManyToManyDestRepository : IEFRepository<ManyToManyDest, ManyToManyDest>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ManyToManyDest?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ManyToManyDest>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}