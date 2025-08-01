using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IC_RequiredCompositeRepository : IEFRepository<C_RequiredComposite, C_RequiredComposite>
    {
        [IntentManaged(Mode.Fully)]
        Task<C_RequiredComposite?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<C_RequiredComposite?> FindByIdAsync(Guid id, Func<IQueryable<C_RequiredComposite>, IQueryable<C_RequiredComposite>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<C_RequiredComposite>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}