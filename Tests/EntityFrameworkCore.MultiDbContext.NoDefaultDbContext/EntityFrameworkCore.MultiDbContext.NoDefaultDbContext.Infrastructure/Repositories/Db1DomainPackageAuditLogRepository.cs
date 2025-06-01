using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Entities;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class Db1DomainPackageAuditLogRepository : RepositoryBase<Db1DomainPackageAuditLog, Db1DomainPackageAuditLog, Db1DbContext>, IDb1DomainPackageAuditLogRepository
    {
        public Db1DomainPackageAuditLogRepository(Db1DbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<Db1DomainPackageAuditLog?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Db1DomainPackageAuditLog?> FindByIdAsync(
            int id,
            Func<IQueryable<Db1DomainPackageAuditLog>, IQueryable<Db1DomainPackageAuditLog>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<Db1DomainPackageAuditLog>> FindByIdsAsync(
            int[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}