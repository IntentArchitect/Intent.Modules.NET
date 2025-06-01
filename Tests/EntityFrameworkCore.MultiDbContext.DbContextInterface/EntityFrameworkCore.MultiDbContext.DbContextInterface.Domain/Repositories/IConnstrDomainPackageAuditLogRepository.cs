using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.DbContextInterface.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IConnstrDomainPackageAuditLogRepository : IEFRepository<ConnstrDomainPackageAuditLog, ConnstrDomainPackageAuditLog>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ConnstrDomainPackageAuditLog?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<ConnstrDomainPackageAuditLog?> FindByIdAsync(int id, Func<IQueryable<ConnstrDomainPackageAuditLog>, IQueryable<ConnstrDomainPackageAuditLog>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<ConnstrDomainPackageAuditLog>> FindByIdsAsync(int[] ids, CancellationToken cancellationToken = default);
    }
}