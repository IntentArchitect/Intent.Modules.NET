using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.Nullability;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.Nullability
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface INullabilityPeerRepository : IEFRepository<NullabilityPeer, NullabilityPeer>
    {
        [IntentManaged(Mode.Fully)]
        Task<NullabilityPeer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<NullabilityPeer>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}