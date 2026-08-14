using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SplitQueries.PostgreSQL.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SplitQueries.PostgreSQL.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IGeometryTypeRepository : IEFRepository<GeometryType, GeometryType>
    {
        [IntentManaged(Mode.Fully)]
        Task<GeometryType?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<GeometryType?> FindByIdAsync(Guid id, Func<IQueryable<GeometryType>, IQueryable<GeometryType>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<GeometryType>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}