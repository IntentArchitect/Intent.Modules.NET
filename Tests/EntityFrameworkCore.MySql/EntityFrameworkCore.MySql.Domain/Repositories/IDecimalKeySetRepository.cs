using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDecimalKeySetRepository : IEFRepository<DecimalKeySet, DecimalKeySet>
    {
        [IntentManaged(Mode.Fully)]
        Task<DecimalKeySet?> FindByIdAsync(decimal id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<DecimalKeySet?> FindByIdAsync(decimal id, Func<IQueryable<DecimalKeySet>, IQueryable<DecimalKeySet>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<DecimalKeySet>> FindByIdsAsync(decimal[] ids, CancellationToken cancellationToken = default);
    }
}