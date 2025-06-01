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
    public interface IAppDbDomainPackageAuditLogRepository : IEFRepository<AppDbDomainPackageAuditLog, AppDbDomainPackageAuditLog>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AppDbDomainPackageAuditLog?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AppDbDomainPackageAuditLog?> FindByIdAsync(int id, Func<IQueryable<AppDbDomainPackageAuditLog>, IQueryable<AppDbDomainPackageAuditLog>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AppDbDomainPackageAuditLog>> FindByIdsAsync(int[] ids, CancellationToken cancellationToken = default);
    }
}