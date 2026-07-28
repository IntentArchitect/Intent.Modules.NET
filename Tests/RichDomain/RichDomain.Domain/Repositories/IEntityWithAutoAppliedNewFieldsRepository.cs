using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using RichDomain.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace RichDomain.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IEntityWithAutoAppliedNewFieldsRepository : IEFRepository<IEntityWithAutoAppliedNewFields, EntityWithAutoAppliedNewFields>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<IEntityWithAutoAppliedNewFields?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<IEntityWithAutoAppliedNewFields?> FindByIdAsync(Guid id, Func<IQueryable<EntityWithAutoAppliedNewFields>, IQueryable<EntityWithAutoAppliedNewFields>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<IEntityWithAutoAppliedNewFields>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}