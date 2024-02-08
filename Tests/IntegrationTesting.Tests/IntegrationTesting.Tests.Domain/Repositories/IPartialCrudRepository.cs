using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace IntegrationTesting.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPartialCrudRepository : IEFRepository<PartialCrud, PartialCrud>
    {
        [IntentManaged(Mode.Fully)]
        Task<PartialCrud?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<PartialCrud>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}