using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace IntegrationTesting.Tests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IHasDateOnlyFieldRepository : IEFRepository<HasDateOnlyField, HasDateOnlyField>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<HasDateOnlyField?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<HasDateOnlyField?> FindByIdAsync(Guid id, Func<IQueryable<HasDateOnlyField>, IQueryable<HasDateOnlyField>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<HasDateOnlyField>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}