using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Postgres.Domain.Entities.Geometry;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.Domain.Repositories.Geometry
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IGeometryTypeRepository : IEFRepository<GeometryType, GeometryType>
    {
        [IntentManaged(Mode.Fully)]
        Task<GeometryType?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<GeometryType>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}