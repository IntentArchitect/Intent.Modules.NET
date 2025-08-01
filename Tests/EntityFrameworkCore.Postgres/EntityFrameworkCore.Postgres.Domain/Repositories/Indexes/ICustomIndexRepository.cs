using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomIndexRepository : IEFRepository<CustomIndex, CustomIndex>
    {
        [IntentManaged(Mode.Fully)]
        Task<CustomIndex?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<CustomIndex?> FindByIdAsync(Guid id, Func<IQueryable<CustomIndex>, IQueryable<CustomIndex>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<CustomIndex>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}