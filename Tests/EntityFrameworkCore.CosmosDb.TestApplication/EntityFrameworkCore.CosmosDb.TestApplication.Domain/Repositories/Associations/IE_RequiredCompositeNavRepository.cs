using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IE_RequiredCompositeNavRepository : IEFRepository<E_RequiredCompositeNav, E_RequiredCompositeNav>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);

        [IntentManaged(Mode.Fully)]
        Task<E_RequiredCompositeNav?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<E_RequiredCompositeNav?> FindByIdAsync(Guid id, Func<IQueryable<E_RequiredCompositeNav>, IQueryable<E_RequiredCompositeNav>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<E_RequiredCompositeNav>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}