using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAppDbContextDomainPackageAuditLogRepository : IEFRepository<AppDbContextDomainPackageAuditLog, AppDbContextDomainPackageAuditLog>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AppDbContextDomainPackageAuditLog?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AppDbContextDomainPackageAuditLog?> FindByIdAsync(int id, Func<IQueryable<AppDbContextDomainPackageAuditLog>, IQueryable<AppDbContextDomainPackageAuditLog>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AppDbContextDomainPackageAuditLog>> FindByIdsAsync(int[] ids, CancellationToken cancellationToken = default);
    }
}