using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Entities.Pagination;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FastEndpointsTest.Domain.Repositories.Pagination
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPersonEntryRepository : IEFRepository<PersonEntry, PersonEntry>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PersonEntry?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PersonEntry?> FindByIdAsync(Guid id, Func<IQueryable<PersonEntry>, IQueryable<PersonEntry>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<PersonEntry>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}