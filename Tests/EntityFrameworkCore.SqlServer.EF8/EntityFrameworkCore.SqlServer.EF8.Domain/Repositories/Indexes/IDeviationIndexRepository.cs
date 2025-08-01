using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF8.Domain.Entities.Indexes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Repositories.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDeviationIndexRepository : IEFRepository<DeviationIndex, DeviationIndex>
    {
        [IntentManaged(Mode.Fully)]
        Task<DeviationIndex?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<DeviationIndex?> FindByIdAsync(Guid id, Func<IQueryable<DeviationIndex>, IQueryable<DeviationIndex>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<DeviationIndex>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}