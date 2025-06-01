using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDb3DomainPackageAuditLogRepository : IEFRepository<Db3DomainPackageAuditLog, Db3DomainPackageAuditLog>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Db3DomainPackageAuditLog?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Db3DomainPackageAuditLog?> FindByIdAsync(int id, Func<IQueryable<Db3DomainPackageAuditLog>, IQueryable<Db3DomainPackageAuditLog>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Db3DomainPackageAuditLog>> FindByIdsAsync(int[] ids, CancellationToken cancellationToken = default);
    }
}