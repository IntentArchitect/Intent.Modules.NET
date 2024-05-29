using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.CompositeKeys;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.CompositeKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IWithCompositeKeyRepository : IEFRepository<WithCompositeKey, WithCompositeKey>
    {
        [IntentManaged(Mode.Fully)]
        Task<WithCompositeKey?> FindByIdAsync((Guid Key1Id, Guid Key2Id) id, CancellationToken cancellationToken = default);
    }
}