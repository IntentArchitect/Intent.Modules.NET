using System.Linq.Expressions;
using EntityFrameworkCore.SqlServer.EF11.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITemporalProductRepository : IEFRepository<TemporalProduct, TemporalProduct>
    {
        Task<List<TemporalHistory<TemporalProduct>>> FindHistoryAsync(TemporalHistoryQueryOptions historyOptions, CancellationToken cancellationToken = default);
        Task<List<TemporalHistory<TemporalProduct>>> FindHistoryAsync(TemporalHistoryQueryOptions historyOptions, Expression<Func<TemporalProduct, bool>> filterExpression, CancellationToken cancellationToken = default);
        Task<List<TemporalHistory<TemporalProduct>>> FindHistoryAsync(TemporalHistoryQueryOptions historyOptions, Expression<Func<TemporalProduct, bool>> filterExpression, Func<IQueryable<TemporalProduct>, IQueryable<TemporalProduct>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TemporalProduct?> FindByIdAsync(Guid id1, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<TemporalProduct?> FindByIdAsync(Guid id1, Func<IQueryable<TemporalProduct>, IQueryable<TemporalProduct>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<TemporalProduct>> FindByIdsAsync(Guid[] id1s, CancellationToken cancellationToken = default);
    }
}