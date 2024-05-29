using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.DDD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.DDD
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICameraRepository : IEFRepository<Camera, Camera>
    {
        [IntentManaged(Mode.Fully)]
        Task<Camera?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Camera>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}