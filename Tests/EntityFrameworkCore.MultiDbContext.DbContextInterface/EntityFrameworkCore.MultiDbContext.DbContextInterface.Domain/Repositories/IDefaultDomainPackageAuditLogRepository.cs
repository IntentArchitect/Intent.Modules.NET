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
    public interface IDefaultDomainPackageAuditLogRepository : IEFRepository<DefaultDomainPackageAuditLog, DefaultDomainPackageAuditLog>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<DefaultDomainPackageAuditLog?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<DefaultDomainPackageAuditLog?> FindByIdAsync(int id, Func<IQueryable<DefaultDomainPackageAuditLog>, IQueryable<DefaultDomainPackageAuditLog>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<DefaultDomainPackageAuditLog>> FindByIdsAsync(int[] ids, CancellationToken cancellationToken = default);
    }
}