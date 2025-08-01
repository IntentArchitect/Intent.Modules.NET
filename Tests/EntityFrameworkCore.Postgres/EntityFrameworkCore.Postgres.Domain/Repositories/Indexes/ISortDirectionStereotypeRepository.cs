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
    public interface ISortDirectionStereotypeRepository : IEFRepository<SortDirectionStereotype, SortDirectionStereotype>
    {
        [IntentManaged(Mode.Fully)]
        Task<SortDirectionStereotype?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<SortDirectionStereotype?> FindByIdAsync(Guid id, Func<IQueryable<SortDirectionStereotype>, IQueryable<SortDirectionStereotype>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<SortDirectionStereotype>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}