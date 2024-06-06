using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.ConventionBasedEventPublishing;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Repositories.ConventionBasedEventPublishing
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IIntegrationTriggeringRepository : IEFRepository<IntegrationTriggering, IntegrationTriggering>
    {
        [IntentManaged(Mode.Fully)]
        Task<IntegrationTriggering?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IntegrationTriggering>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}