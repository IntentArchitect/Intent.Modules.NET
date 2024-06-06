using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.BasicMappingMapToValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.BasicMappingMapToValueObjects
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ISubmissionRepository : IEFRepository<Submission, Submission>
    {
        [IntentManaged(Mode.Fully)]
        Task<Submission?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Submission>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}