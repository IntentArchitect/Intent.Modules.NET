using CqrsSpike.Publisher.Domain.Entities;

namespace CqrsSpike.Publisher.Domain.Repositories
{
    public interface IAuditEntryRepository : IEFRepository<AuditEntry, AuditEntry>
    {
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        Task<AuditEntry?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<AuditEntry?> FindByIdAsync(Guid id, Func<IQueryable<AuditEntry>, IQueryable<AuditEntry>> queryOptions, CancellationToken cancellationToken = default);
        Task<List<AuditEntry>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}