using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.SynchronousRepositories.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFramework.SynchronousRepositories.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IInvoiceRepository : IEFRepository<Invoice, Invoice>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        TProjection? FindByIdProjectTo<TProjection>(Guid id);
        [IntentManaged(Mode.Fully)]
        Task<Invoice?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Invoice?> FindByIdAsync(Guid id, Func<IQueryable<Invoice>, IQueryable<Invoice>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Invoice>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Invoice? FindById(Guid id);
        [IntentManaged(Mode.Fully)]
        Invoice? FindById(Guid id, Func<IQueryable<Invoice>, IQueryable<Invoice>> queryOptions);
        [IntentManaged(Mode.Fully)]
        List<Invoice> FindByIds(Guid[] ids);
    }
}