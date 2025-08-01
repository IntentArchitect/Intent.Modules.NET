using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MySql.Domain.Entities.Associations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Domain.Repositories.Associations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IR_SourceNameDiffRepository : IEFRepository<R_SourceNameDiff, R_SourceNameDiff>
    {
        [IntentManaged(Mode.Fully)]
        Task<R_SourceNameDiff?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<R_SourceNameDiff?> FindByIdAsync(Guid id, Func<IQueryable<R_SourceNameDiff>, IQueryable<R_SourceNameDiff>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<R_SourceNameDiff>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}