using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Entities;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Repositories;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AlternateConnStrDefaultDbDomainPackageAuditLogRepository : RepositoryBase<AlternateConnStrDefaultDbDomainPackageAuditLog, AlternateConnStrDefaultDbDomainPackageAuditLog, AlternateConnStrDefaultDbDbContext>, IAlternateConnStrDefaultDbDomainPackageAuditLogRepository
    {
        public AlternateConnStrDefaultDbDomainPackageAuditLogRepository(AlternateConnStrDefaultDbDbContext dbContext,
            IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<AlternateConnStrDefaultDbDomainPackageAuditLog?> FindByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<AlternateConnStrDefaultDbDomainPackageAuditLog?> FindByIdAsync(
            int id,
            Func<IQueryable<AlternateConnStrDefaultDbDomainPackageAuditLog>, IQueryable<AlternateConnStrDefaultDbDomainPackageAuditLog>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<AlternateConnStrDefaultDbDomainPackageAuditLog>> FindByIdsAsync(
            int[] ids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}