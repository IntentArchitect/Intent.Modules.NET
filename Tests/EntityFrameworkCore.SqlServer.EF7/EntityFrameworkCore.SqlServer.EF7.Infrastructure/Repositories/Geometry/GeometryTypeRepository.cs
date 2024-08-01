using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.EF7.Domain.Entities.Geometry;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories;
using EntityFrameworkCore.SqlServer.EF7.Domain.Repositories.Geometry;
using EntityFrameworkCore.SqlServer.EF7.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Repositories.Geometry
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GeometryTypeRepository : RepositoryBase<GeometryType, GeometryType, ApplicationDbContext>, IGeometryTypeRepository
    {
        public GeometryTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<GeometryType?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<GeometryType>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}